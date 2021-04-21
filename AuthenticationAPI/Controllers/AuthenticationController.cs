using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Authenticators;

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

        [HttpGet]
        [Authorize]
        public ActionResult<string> Get()
        {
            this.User.Claims.ToList().ForEach(claim =>
            {
                System.Console.WriteLine("key: " + claim.Type);
                System.Console.WriteLine("claim: " + claim.Value);
            });
            return Ok(new { access = "granted" });
        }

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
                return new ForbidResult();
            }
            return dictionary;
        }


        [HttpGet("isadmin")]
        [Authorize("manage:website")]
        public ActionResult IsAdmin()
        {
            System.Console.WriteLine("access for admin granted");
            return Ok(new { access = "granted" });
        }


        [HttpGet("ismoderator")]
        [Authorize("manage:forums")]
        public ActionResult IsModerator()
        {
            System.Console.WriteLine("access for moderator granted");
            return Ok(new { access = "granted" });
        }

    }
}
