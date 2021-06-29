using IdentityApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity {
    [AllowAnonymous]
    public class UserAccountCompleteExternalModel : UserPageModel {
        public UserAccountCompleteExternalModel(
                UserManager<IdentityUser> usrMgr,
                SignInManager<IdentityUser> signMgr,
                TokenUrlEncoderService encoder) {
            UserManager = usrMgr;
            SignInManager = signMgr;
            TokenUrlEncoder = encoder;
        }
        public UserManager<IdentityUser> UserManager { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public TokenUrlEncoderService TokenUrlEncoder { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public async Task<string> ExternalProvider() =>
            (await UserManager.GetLoginsAsync(IdentityUser))
                .FirstOrDefault()?.ProviderDisplayName;
        public async Task<IActionResult> OnPostAsync(string provider) {
            IdentityUser = await UserManager.FindByEmailAsync(Email);
            string decodedToken = TokenUrlEncoder.DecodeToken(Token);
            // The basic approach is the same as for self-registration, but some changes are required to fit into the administrator-led model. First, this page has to validate the token that was sent to the user when the account was created. The token is intended for use with the method that resets a password but can be validated on its own, albeit awkwardly, with the user manager’s VerifyUserTokenAsync method.
            // VerifyUserTokenAsync(user, provider, purpose, token)
            // This method returns true if the specified token is valid for the specified user and purpose and was created using the specified generator.
            // Create.cshtml.cs using 
            //             string token = await UserManager.GeneratePasswordResetTokenAsync(user);
            bool valid = await UserManager.VerifyUserTokenAsync(
                IdentityUser,
                UserManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<IdentityUser>.ResetPasswordTokenPurpose, decodedToken);
            if (!valid) {
                return Error("Invalid token");
            }
            string callbackUrl = Url.Page("UserAccountCompleteExternal",
                "Callback", new { Email, Token });
            AuthenticationProperties props =
               SignInManager.ConfigureExternalAuthenticationProperties(
                   provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }
        public async Task<IActionResult> OnGetCallbackAsync() {
            ExternalLoginInfo info = await SignInManager.GetExternalLoginInfoAsync();
            string email = info?.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) {
                return Error("External service has not provided an email address.");
            } else if ((IdentityUser =
                    await UserManager.FindByEmailAsync(email)) == null) {
                return Error("Your email address doesn't match.");
            }
            IdentityResult result
                = await UserManager.AddLoginAsync(IdentityUser, info);
            if (!result.Succeeded) {
                return Error("Cannot store external login.");
            }
            return RedirectToPage(new { id = IdentityUser.Id });
        }
        public async Task<IActionResult> OnGetAsync(string id) {
            if ((id == null
                || (IdentityUser = await UserManager.FindByIdAsync(id)) == null)
                && !TempData.ContainsKey("errorMessage")) {
                return RedirectToPage("SignIn");
            }
            return Page();
        }
        private IActionResult Error(string err) {
            TempData["errorMessage"] = err;
            return RedirectToPage();
        }
    }
}
