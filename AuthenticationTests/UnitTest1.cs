using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthenticationAPI.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Http;
using AuthenticationAPI;

namespace AuthenticationTests
{
    public class UnitTest1
    {
        //protected readonly WebApiTesterFactory factory;
        protected HttpClient client;
        //protected dynamic token;

        public UnitTest1()
        {
            var factory = new WebApplicationFactory<Startup>();
            client = factory.CreateClient();
        }

        [Fact]
        public async Task AuthenticationGetEndpointUnauthorizedTest()
        {
            var response = await client.GetAsync("authentication");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AuthenticationUserdataEndpointUnauthorizedTest()
        {
            var response = await client.GetAsync("authentication/userdata");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AuthenticationGetEndpointAuthorizedTest()
        {
            string usertype = "reg";
            await AuthenticateAsync(usertype);
            var response = await client.GetAsync("authentication");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationUserdataEndpointAuthorizedTest()
        {
            string usertype = "reg";
            await AuthenticateAsync(usertype);
            var response = await client.GetAsync("authentication/userdata");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationIsAdminEndpointUnauthorizedModTest()
        {
            string userType = "mod";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/isadmin");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AuthenticationIsAdminEndpointUnauthorizedRegTest()
        {
            string userType = "reg";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/isadmin");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AuthenticationIsAdminEndpointAuthorizedTest()
        {
            string userType = "admin";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/isadmin");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationIsModEndpointUnauthorizedRegTest()
        {
            string userType = "reg";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/ismoderator");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AuthenticationIsModEndpointAuthorizedModTest()
        {
            string userType = "mod";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/ismoderator");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationIsModEndpointAuthorizedAdminTest()
        {
            string userType = "admin";
            await AuthenticateAsync(userType);
            var response = await client.GetAsync("authentication/ismoderator");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationTestAdminRoleManagement()
        {
            string userType = "reg";
            await AuthenticateAsync(userType);
            dynamic content = new StringContent("");
            HttpResponseMessage response = await client.PostAsync("authentication/role/Admin", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var response2 = await client.DeleteAsync("authentication/role/Admin");
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task AuthenticationTestModeratorRoleManagement()
        {
            string userType = "reg";
            await AuthenticateAsync(userType);
            dynamic content = new StringContent("");
            HttpResponseMessage response = await client.PostAsync("authentication/role/Moderator", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var response2 = await client.DeleteAsync("authentication/role/Moderator");
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        protected async Task AuthenticateAsync(string userType)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync(userType));
        }

        private async Task<string> GetJwtAsync(string userType)
        {
            string token = "";
            switch (userType)
            {
                case "reg":
                    token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ill4MlRZT1Fja1hwUUlaQ3lJMVNHOSJ9.eyJpc3MiOiJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjA4MmIzZWU1OTlmODMwMDZhZDYxYWJiIiwiYXVkIjpbImh0dHBzOi8vY2luZXBoaWxpYWNzLWFwaS8iLCJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjE5MTc4NDg0LCJleHAiOjE2MTkyNjQ4ODQsImF6cCI6InVEem05QldTYTBKM2VQdWZIbndPanh6S1dPMmhwVzVQIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInBlcm1pc3Npb25zIjpbXX0.WlFITObKwT7Nvs8vvKvcldWMGI0TPqNE83wLttnVB0lKRA7hrAdMR3y_2l-yh2BQADkRQc2FjCBHgGGjBDJdyZIgf21i71Prafdky1dM74KCCAqHTg5p94qIvcvjFY9F6f7fvob-E24bwSByPj9dmO2InY75_EkUTn9hDfFk52DlwfcGZDxMTK5sMwK_Jnt-5BfH93nMH3OtmmcHlXo6jwABFwHgvS_0-3YNVovTd9whUbjc7jWXpnx137TFugLednoSKgEd8y_J542S7YvHUE9eaUPWqt81rff6pm_fNKCLODeQ5FeKbmXbnNLpiKFtYJmL8Bh1DNKmOOOVRnS8YA";
                    break;
                case "admin":
                    token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ill4MlRZT1Fja1hwUUlaQ3lJMVNHOSJ9.eyJpc3MiOiJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjA4MDU3NGVjOWM5M2UwMDZiODE4ZWU5IiwiYXVkIjpbImh0dHBzOi8vY2luZXBoaWxpYWNzLWFwaS8iLCJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjE5MTMzNDg3LCJleHAiOjE2MTkyMTk4ODcsImF6cCI6InVEem05QldTYTBKM2VQdWZIbndPanh6S1dPMmhwVzVQIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInBlcm1pc3Npb25zIjpbIm1hbmFnZTphd2Vic2l0ZSIsIm1hbmFnZTpmb3J1bXMiXX0.kgm46bSLjxUOPoEcBYJTpWM_UOEM8zpskMIJPaGUDgd49C0yyrNlgreEs7AolQvWZdTb_yzxjqkxs6kxG5FlH92tTwFR0-UxSINIolTuJRmWmrF36CMqyQo8jFR-zlUCsfGcJM-2zBX9YwDNlQ8ws67a71TnNVtnN6-sMRZKCbYQT2Lo6Y4hQoYbwHfdtcRIXwF-ibKyytd5uflU8BkrMn62Zf5D5F5ydJ9R8X5N-ZZcXHg5tZ_zEvbVM7BWvM9nN9ZTri5ou8slKh4Br1fVW99D25j3oiMxVMlI6ztVR3C6BeSSNVIGb1tLK2Ha_SDq3uKX6bRGFQAgPEeq1uQ4vA";
                    break;
                case "mod":
                    token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ill4MlRZT1Fja1hwUUlaQ3lJMVNHOSJ9.eyJpc3MiOiJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NjA4MjJmMjU1YzcxNGMwMDZmYjA4YjJhIiwiYXVkIjpbImh0dHBzOi8vY2luZXBoaWxpYWNzLWFwaS8iLCJodHRwczovL2NpbmVwaGlsaWFjcy51cy5hdXRoMC5jb20vdXNlcmluZm8iXSwiaWF0IjoxNjE5MTQ0NjAyLCJleHAiOjE2MTkyMzEwMDIsImF6cCI6InVEem05QldTYTBKM2VQdWZIbndPanh6S1dPMmhwVzVQIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInBlcm1pc3Npb25zIjpbIm1hbmFnZTpmb3J1bXMiXX0.YbtmJ_3hYuNS2am1v3Bbl-pk03_-OgSZX098Hvg-6E_8BAYnT0NemHqL5O6-_1axqyTdDimADzGNwT40dgBP6P4bmRJLkqRHUheTUkDy6LKhjgwj58S0NwWkcaHfTsggqIhkIXB3L8-DsiLIvEvjBmYfD9pQDFWn57F6BayN1QfpJ8qdlLA6uO8_HJklu2T8RSZoCyHY8glfWCfEUgOqGVfBYJcU_xMq91llVVnTOifyYI2dU0ul5m0nHS0BdkspOcrQylF62GyXZMIEe8sZEtNE0hZ922MngfO33Ok-vai3crVPOZf0BALmMPS8r6KgAFezVLTv6bum_KT2dZi2xw";
                    break;

            }

            return token;
        }

    }
}
