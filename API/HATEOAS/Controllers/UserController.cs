using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefArc.Api.HATEOAS.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        // GET api/values
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var name = User.Identity.Name;
            var claims = User.Claims;

            //TODO: Make it a json API
            return new string[] { name, "claims: " + string.Join(", ", claims) };
        }
    }
}
