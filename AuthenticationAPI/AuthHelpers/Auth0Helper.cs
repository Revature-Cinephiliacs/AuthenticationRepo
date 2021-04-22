using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace AuthenticationAPI.AuthHelpers
{
    public class Auth0Helper
    {
        private readonly IConfiguration _configuration;
        string baseUrl;

        public Auth0Helper(IConfiguration _configuration)
        {
            this._configuration = _configuration;
            this.baseUrl = $"https://{_configuration["Auth0:Domain"]}";
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
        public async Task<IRestResponse> Sendrequest(string urlExtension, Method method, string token)
        {
            System.Console.WriteLine("baseUrl+urlExtension");
            System.Console.WriteLine(baseUrl + urlExtension);
            var client = new RestClient(baseUrl + urlExtension);
            client.Timeout = -1;
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", token);
            IRestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine("response.Content");
            Console.WriteLine(response.Content);
            return response;
        }
    }
}