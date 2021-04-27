using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationAPI.AuthHelpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthenticationAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string permissionName = "permissions";
            services.AddScoped<Auth0Helper>(s => new Auth0Helper(Configuration, permissionName));

            services.AddControllers();


            services.AddCors(options =>
            {
                options.AddPolicy(name: "_corsPolicy",
                    builder => builder
                    .WithOrigins(
                        "http://20.189.30.176/", // deployed reviews backend
                        "http://20.45.6.142/", // deployed forums backend
                        "http://20.45.0.67/", // deployed groups backend
                        "http://20.189.29.112/", // deployed adminTools backend
                        "http://20.189.28.203/", // deployed recommendations backend
                        "http://20.45.2.119/", // deployed users backend
                        "http://20.94.153.81/", // deployed movies backend
                        "http://20.94.137.143/" // deployed angular frontend
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            });

            // for authentication
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            // string domain = $"https://authenticationapi/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            // for permissions/authorization
            string adminPermisson = "manage:awebsite";
            string modPermission = "manage:forums";
            services.AddAuthorization(options =>
            {
                options.AddPolicy(adminPermisson, policy => policy.Requirements.Add(new HasScopeRequirement(adminPermisson, domain)));
                options.AddPolicy(modPermission, policy => policy.Requirements.Add(new HasScopeRequirement(modPermission, domain)));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthenticationAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthenticationAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("_corsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
