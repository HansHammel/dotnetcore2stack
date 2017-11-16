using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IS4ProtectedAPI.Controllers
{
    [Produces("application/json")]
    [Route("identity")]
    [Authorize]
    public class ClientIdentityController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }

    /*
    [Produces("application/json")]
    [Route("api/ClientIdentity")]
    public class ClientIdentityController : Controller
    {
    }
    */
}
