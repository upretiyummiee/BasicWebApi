using BasicWebApi.Data.IdentityData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<IdentityUserInherit> _userManager;
        private readonly SignInManager<IdentityUserInherit> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUserInherit> userManager,
            SignInManager<IdentityUserInherit> signInManager,
            IConfiguration configuration) {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("SignIn")]
        [RequireHttps()]
        public async Task<IActionResult> SignIn(SignInUser sign)
        {
            if (sign is null)
            {
                return BadRequest("No data provided. Object is null.");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(sign.UserName, sign.Password, true, false);
                if (result.Succeeded)
                {
                    ////JWT Token Generation
                    var claims = new[]
                    {
                        new Claim("Username", sign.UserName)
                    };

                    var signingcredentials = new SigningCredentials(new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration.GetSection("key").ToString())),
                        SecurityAlgorithms.HmacSha512);

                    var tokendescriptor = new SecurityTokenDescriptor {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddMinutes(4),
                        SigningCredentials = signingcredentials
                    };

                    var tokenhandler = new JwtSecurityTokenHandler();
                    var token = tokenhandler.CreateToken(tokendescriptor);
                    return Ok(tokenhandler.WriteToken(token));
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterUser register) {

            if (register is null)
            {
                return BadRequest("No data or sufficient data provided. Object is null.");
            }
            else {

                var identityUser = new IdentityUserInherit
                {
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    UserName = register.UserName
                };

                if (ModelState.IsValid)
                {
                    if (register.Password == register.ConfirmPassword)
                    {
                        var result = await _userManager.CreateAsync(identityUser, register.Password);
                        if (result.Succeeded)
                        {
                            return Created(string.Empty, register);
                        }
                        else
                        {
                            return Problem(result.Errors.First().Description, null, 503);
                        }
                    }
                    else
                    {
                        return BadRequest("Confirm password validation error.");
                    }
                }
                else
                {
                    return ValidationProblem(ModelState);
                }
            }
        }

        [HttpPost("signout")]
        public async Task<IActionResult> Out()
        {
            var req = User.Identity.IsAuthenticated;
            var type = User.Identity.AuthenticationType;
            if (!req) {
                return Unauthorized();
            }
            await _signInManager.SignOutAsync();
            return Ok();
        }

    }
}
