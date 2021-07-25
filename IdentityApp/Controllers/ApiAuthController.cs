using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System;
using System.Text;
using System.Security.Claims;
using IdentityApp.Models;
namespace IdentityApp.Controllers {
    [ApiController]
    [Route("/api/auth")]
    public class ApiAuthController: ControllerBase {
        private SignInManager<IdentityUser> SignInManager;
        private UserManager<IdentityUser> UserManager;
        private IConfiguration Configuration;
        public ApiAuthController(SignInManager<IdentityUser> signMgr,
                UserManager<IdentityUser> usrMgr,
                IConfiguration config) {
            SignInManager = signMgr;
            UserManager = usrMgr;
            Configuration = config;
        }
/// <summary>
/// The sign-in process has two key steps. The first step is to validate the password provided by the user. To do this without signing the user into the application, the sign-in manager’s CheckPasswordSignInAsync method is used. This method operates on IdentityUser objects, which are obtained from the store from the user manager.
/// If the correct password has been provided, a token is created and sent back in the response. A SecurityTokenDescriptor object is created with the properties described in Table 12-5.
/// </summary>
/// <param name="user"></param>
/// <returns></returns>
        [HttpPost("signin")]
        public async Task<object> ApiSignIn(
                [FromBody] SignInCredentials creds) {
            Console.WriteLine($"Email:{creds.Email}, Password:{creds.Password}");
            IdentityUser user = await UserManager.FindByEmailAsync(creds.Email);
            SignInResult result = await SignInManager.CheckPasswordSignInAsync(user,
                creds.Password, true);
            if (result.Succeeded) {
                // The authentication handler set up by the AddJwtBearer method will validate tokens, but the application is responsible for generating them.
                var temp = (await SignInManager.CreateUserPrincipalAsync(user)).Identities;
                foreach(var t in temp){
                    Console.WriteLine($"Name:{t.Name}, RoleClaimType:{t.RoleClaimType}, AuthenticationType :{t.AuthenticationType}, IsAuthenticated:{t.IsAuthenticated }, NameClaimType:{t.NameClaimType}");
                    Console.WriteLine("#######################");
                    Console.WriteLine("all claims 1");
                    Console.WriteLine("#######################");
                    foreach (Claim c in t.Claims) {
                        Console.WriteLine($"Type:{c.GetDisplayName()}, Value:{c.Value}, Issuer:{c.Issuer}");
                    }    
                    Console.WriteLine("#######################");
                    Console.WriteLine("all claims 2");
                    Console.WriteLine("#######################");
                    foreach (Claim c in t.Claims) {
                        Console.WriteLine($"Type:{c.Type}, Value:{c.Value}, Issuer:{c.Issuer}");
                    }    
                }
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor {
                    Subject = (await SignInManager.CreateUserPrincipalAsync(user)).Identities.First(),
                    Expires = DateTime.Now.AddMinutes(int.Parse(Configuration["JWTBearerTokens:ExpiryMins"])),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTBearerTokens:Key"])),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken secToken = new JwtSecurityTokenHandler().CreateToken(descriptor);
                return new { success = true, token = handler.WriteToken(secToken)};
            }
            return new { success = false  };
        }
//         [HttpPost("signout")]
//         public async Task<IActionResult> ApiSignOut() {
//             await SignInManager.SignOutAsync();
//             return Ok();
//         }
    }
    public class SignInCredentials {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}