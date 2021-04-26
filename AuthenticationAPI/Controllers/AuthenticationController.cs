using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthenticationAPI.AuthHelpers;
using RestSharp;

namespace AuthenticationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Auth0Helper _helper;

        public AuthenticationController(Auth0Helper _helper)
        {
            this._helper = _helper;
        }

        /// <summary>
        /// returns true if request has a valid token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> Get()
        {
            var pers = _helper.ExtractPermissions(this.User);
            return Ok(new { access = "granted", permissions = pers });
        }

        /// <summary>
        /// returns the userdata saved in auth0 if user is signed in
        /// </summary>
        /// <returns></returns>
        [HttpGet("userdata")]
        [Authorize]
        public async Task<ActionResult<Dictionary<string, string>>> GetUserData()
        {
            // if we need to get user data
            // pass to helper
            var dictionary = await _helper.GetUserAuth0Dictionary(this.Request);
            if (dictionary == null)
            {
                return new BadRequestResult();
            }
            return dictionary;
        }

        /// <summary>
        /// adds roleName as a role to the user
        /// </summary>
        /// <param name="roleName">Either Admin or Moderator</param>
        /// <param name="userid">The id of the user we need to add permission to</param>
        /// <returns></returns>
        [HttpPost("role/{roleName}")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult<bool>> AddRole(string roleName, [FromBody] string userid)
        {
            System.Console.WriteLine("rolename: " + roleName);
            return await _helper.ChangeAdminRole(this.Request, Method.POST, roleName, userid);
        }

        /// <summary>
        /// Removes roleName role from the user roles
        /// </summary>
        /// <param name="roleName">Either Admin or Moderator</param>
        /// <param name="userid">The id of the user we need to remove permission from</param>
        /// <returns></returns>
        [HttpDelete("role/{roleName}")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult<bool>> DeleteRole(string roleName, [FromBody] string userid)
        {
            System.Console.WriteLine("rolename: " + roleName);
            return await _helper.ChangeAdminRole(this.Request, Method.DELETE, roleName, userid);
        }
    }
}
