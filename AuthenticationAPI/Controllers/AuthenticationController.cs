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
        public ActionResult Get()
        {
            return Ok();
        }

        [HttpGet("userdata")]
        [Authorize]
        public async Task<ActionResult<Dictionary<string, string>>> GetUserData()
        {
            // if we need to get user data
            // pass to helper
            var dictionary = await _helper.GetUserAuth0Dictionary(this.Request);
            if (dictionary == null)
            {
                return new ForbidResult();
            }
            return dictionary;
        }


    }
}
