//from: https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization
using System;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationAPI.AuthHelpers
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        /// <summary>
        /// creates a check if a requirement exists in the request
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="issuer"></param>
        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }

}
