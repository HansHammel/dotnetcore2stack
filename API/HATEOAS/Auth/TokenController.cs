using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace RefArc.Api.HATEOAS.Filter
{

    [Route("api/token")]
    public class TokenController : Controller
    {
        private ITokenProvider _tokenProvider;

        public TokenController(ITokenProvider tokenProvider) // We'll create this later, don't worry.
        {
            _tokenProvider = tokenProvider;
        }


        [AllowAnonymous]
        [HttpPost]
        //public async Task<IActionResult> Get([FromBody] UserModel model)
        public async Task<JsonWebToken> Get([FromBody] UserModel model)
        {

            User user = model.grant_type == "refresh_token" ? GetUserByToken(model.refresh_token) : GetUserByCredentials(model.username, model.password);

            if (user == null)
                throw new UnauthorizedAccessException("No!");

            int ageInMinutes = 30;  // However long you want...

            DateTime expiry = DateTime.UtcNow.AddMinutes(ageInMinutes);

            var token = new JsonWebToken
            {
                access_token = _tokenProvider.CreateToken(user, expiry),
                expires_in = ageInMinutes * 60
            };

            if (model.grant_type != "refresh_token")
                token.refresh_token = GenerateRefreshToken(user);

            return token;

        }

        /*
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetAlternative([FromBody] UserModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Token failed to generate");

            var user = (model.password == "test" && model.username == "test");

            if (!user) return Unauthorized();

            //Add Claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, "data"),
                new Claim(JwtRegisteredClaimNames.Sub, "data"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("rlyaKithdrYVl6Z80ODU350md")); //Secret
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("me",
                "you",
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new JsonWebToken()
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                expires_in = 600000,
                token_type = "bearer"
            });

        }
        */

        private User GetUserByToken(object refresh_token)
        {
            throw new NotImplementedException();
        }

        private User GetUserByCredentials(object username, object password)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonWebToken Get([FromQuery] string grant_type, [FromQuery] string username, [FromQuery] string password, [FromQuery] string refresh_token)
        {
            //if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) return BadRequest("Token failed to generate");
            //if (String.IsNullOrEmpty(grant_type) || String.IsNullOrEmpty(refresh_token)) return BadRequest("Token failed to generate");

            // Authenticate depending on the grant type.
            User user = grant_type == "refresh_token" ? GetUserByToken(refresh_token) : GetUserByCredentials(username, password);

            if (user == null)
                throw new UnauthorizedAccessException("No!");

            int ageInMinutes = 30;  // However long you want...

            DateTime expiry = DateTime.UtcNow.AddMinutes(ageInMinutes);

            var token = new JsonWebToken
            {
                access_token = _tokenProvider.CreateToken(user, expiry),
                expires_in = ageInMinutes * 60
            };

            if (grant_type != "refresh_token")
                token.refresh_token = GenerateRefreshToken(user);

            return token;
        }

        private User GetUserByToken(string refreshToken)
        {
            // TODO: Check token against your database.
            if (refreshToken == "test")
                return new User { UserName = "test" };

            return null;
        }

        private User GetUserByCredentials(string username, string password)
        {
            // TODO: Check username/password against your database.
            if (username == password)
                return new User { UserName = username };

            return null;
        }

        private string GenerateRefreshToken(User user)
        {
            // TODO: Create and persist a refresh token.
            return "test";
        }
    }
}
