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
    public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected HttpClient client;

        public UnitTest1(CustomWebApplicationFactory<Startup> factory)
        {
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
            await AuthenticateAsync();
            var response = await client.GetAsync("authentication");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticationUserdataEndpointAuthorizedTest()
        {
            await AuthenticateAsync();
            var response = await client.GetAsync("authentication/userdata");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        protected async Task AuthenticateAsync()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", FakeJwtManager.GenerateJwtToken());
        }

    }
}
