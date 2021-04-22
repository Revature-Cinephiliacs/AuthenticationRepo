using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthenticationAPI.AuthHelpers;


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
        public ActionResult<string> Get()
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
            this.User.Claims.ToList().ForEach(claim =>
            {
                System.Console.WriteLine("claim: " + claim.Value);
            });
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
        /// for checking if a request's user has admin permission
        /// </summary>
        /// <returns></returns>
        [HttpGet("isadmin")]
        [Authorize("manage:awebsite")]
        public ActionResult IsAdmin()
        {
            System.Console.WriteLine("access for admin granted");
            return Ok(new { access = "granted" });
        }

        /// <summary>
        /// for checking if a request's user has moderator permission
        /// </summary>
        /// <returns></returns>
        [HttpGet("ismoderator")]
        [Authorize("manage:forums")]
        public ActionResult IsModerator()
        {
            System.Console.WriteLine("access for moderator granted");
            return Ok(new { access = "granted" });
        }

    }
}
