//from: https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace KitchenWeb.Helpers
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        /// <summary>
        /// finds the permission required and returns a success if found
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindAll(c => c.Type == "permissions" && c.Issuer == requirement.Issuer);

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s.Value == requirement.Scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}