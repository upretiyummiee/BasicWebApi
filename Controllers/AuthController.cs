using BasicWebApi.Data.IdentityData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BasicWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<IdentityUserInherit> _userManager;
        private readonly SignInManager<IdentityUserInherit> _signInManager;

        public AuthController(
            UserManager<IdentityUserInherit> userManager,
            SignInManager<IdentityUserInherit> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInUser sign)
        {
            if (sign is null)
            {
                return BadRequest("No data provided. Object is null.");
            }
            else
            {
                var userData = new IdentityUserInherit
                {
                    UserName = sign.UserName
                };

                var result = await _signInManager.PasswordSignInAsync(sign.UserName, sign.Password, true, false);
                if (result.Succeeded)
                {
                    return Ok();
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
                    return BadRequest("Object validation error occured");
                }
            }
        }

        [HttpGet("signout")]
        public async Task<IActionResult> Out()
        {
            var req = User.Identity.IsAuthenticated;
            if (!req) {
                return Unauthorized();
            }
            await _signInManager.SignOutAsync();
            return Ok();
        }

    }
}
