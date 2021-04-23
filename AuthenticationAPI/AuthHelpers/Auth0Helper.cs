using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AuthenticationAPI.AuthHelpers
{
    public class Auth0Helper
    {
        private readonly IConfiguration _configuration;
        string baseUrl;
        private readonly string _permissionName;

        public Auth0Helper(IConfiguration _configuration, string _permissionName)
        {
            this._permissionName = _permissionName;
            this._configuration = _configuration;
            this.baseUrl = $"https://{_configuration["Auth0:Domain"]}";
        }

        /// <summary>
        /// Extracting permissions from the request user. 
        /// this is used to return permissions to any calling api
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> ExtractPermissions(ClaimsPrincipal user)
        {
            List<string> permissions = new List<string>();
            user.Claims.Where(c => c.Type == _permissionName).ToList().ForEach(claim =>
            {
                System.Console.WriteLine("claim type: " + claim.Type + " value: " + claim.Value);
                System.Console.WriteLine("yep");
                permissions.Add(claim.Value);
            });
            return permissions;
        }

        /// <summary>
        /// Extract token from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>token</returns>
        public static string GetTokenFromRequest(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return request.Headers.FirstOrDefault(h => h.Key == "Authorization").Value;
        }

        /// <summary>
        /// Get user dictionary from Auth0 using the current request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A dictionary of user data</returns>
        public async Task<Dictionary<string, string>> GetUserAuth0Dictionary(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            string token = GetTokenFromRequest(request);
            IRestResponse response = await Sendrequest("/userinfo", Method.GET, token);
            System.Console.WriteLine("user data:");
            Console.WriteLine(response.Content);
            System.Console.WriteLine("response status");
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.IsSuccessful);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Send request to Auth0
        /// </summary>
        /// <param name="urlExtension"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <returns>The response of the request</returns>
        public async Task<IRestResponse> Sendrequest(string urlExtension, Method method, string token, dynamic body = null)
        {
            System.Console.WriteLine("baseUrl+urlExtension");
            System.Console.WriteLine(baseUrl + urlExtension);
            var client = new RestClient(baseUrl + urlExtension);
            client.Timeout = -1;
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", token);
            if (body != null)
            {
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            IRestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine("response.Content");
            Console.WriteLine(response.Content);
            return response;
        }

        /// <summary>
        /// Gives "roleName" role when method is POST. <br/>
        /// Removes "roleName" role when method is DELETE
        /// </summary>
        /// <param name="request">The user request</param>
        /// <param name="method">POST or DELETE</param>
        /// <param name="roleName">Currently Available: Admin or Moderator</param>
        /// <returns></returns>
        public async Task<bool> ChangeAdminRole(Microsoft.AspNetCore.Http.HttpRequest request, Method method, string roleName)
        {
            // Admin: rol_pFa6j3aDlDJJDP4d
            // Moderator: rol_XMHQtgKAXqlfj9qc
            if (roleName != "Admin" && roleName != "Moderator")
            {
                return false;
            }
            var dictionary = await this.GetUserAuth0Dictionary(request);
            var mtok = await this.GetMachineToken();
            System.Console.WriteLine("machine token: " + mtok);
            var roles = JsonConvert.DeserializeObject<Role[]>((await Sendrequest("/api/v2/roles", Method.GET, mtok)).Content);
            var r = roles.ToList().Where(r => r.name == "Admin").FirstOrDefault();
            var roleID = r.id;
            dynamic body = new
            {
                roles = new List<string> { roleID }
            };
            System.Console.WriteLine("body serialized:");
            System.Console.WriteLine(JsonConvert.SerializeObject(body));
            if (dictionary == null)
            {
                return false;
            }
            var response = await Sendrequest($"/api/v2/users/{dictionary["sub"]}/roles", method, mtok, body);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Get the machine to machine type of appilcation (In Auth0) token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMachineToken()
        {
            var client = new RestClient("https://cinephiliacs.us.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\""
            + _configuration["Auth0:machine_id"]
            + "\",\"client_secret\":\""
            + _configuration["Auth0:client_secret"]
            + "\",\"audience\":\""
            + baseUrl + "/api/v2/"
            + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            var tokResp = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
            string token = tokResp.token_type + " " + tokResp.access_token;
            return token;
        }

        /// <summary>
        /// a class to recieve the machine token
        /// </summary>
        class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
        }

        /// <summary>
        /// A class for recieving the Role from Auth0
        /// </summary>
        class Role
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
    }
}