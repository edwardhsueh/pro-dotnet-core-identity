using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity {
    [AllowAnonymous]
    public class SignUpExternalModel : UserPageModel {
        public SignUpExternalModel(UserManager<IdentityUser> usrMgr,
                SignInManager<IdentityUser> signMgr) {
            UserManager = usrMgr;
            SignInManager = signMgr;
        }
        public UserManager<IdentityUser> UserManager { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public async Task<string> ExternalProvider() =>
            (await UserManager.GetLoginsAsync(IdentityUser))
            .FirstOrDefault()?.ProviderDisplayName;
// The sign-in manager’s ConfigureExternalAuthenticationProperties method is called to create an AuthenticationProperties object that will authenticate the user with the selected external provider. These properties are configured with a callback URL, which will be called when the user has authenticated themselves with their chosen service. The AuthenticationProperties object is used to create a challenge response, which will start the authentication process and redirect the user to the selected service.

        public IActionResult OnPost(string provider) {
            string callbackUrl = Url.Page("SignUpExternal", "Callback");
             AuthenticationProperties props = SignInManager.ConfigureExternalAuthenticationProperties(
                    provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }

        // When the user has been authenticated, the OnGetCallbackAsync method will be invoked. The user’s external information is obtained using the sign-in manager’s GetExternalLoginInfoAsync method, which returns an ExternalLoginInfo object. The ExternalLoginInfo.Principal property returns a ClaimsPrincipal object that contains the user’s account information, expressed as a series of claims. The user’s email address is obtained by finding the first claim with the ClaimType.Email type, like this:
        // Once I have an email address, I use it to create a new IdentityUser object and add it to the user store. I then use the FindByEmailAsync method so that I am working with the stored version of the object, including the properties that are generated automatically during the storage process. I call the user manager’s AddLoginAsync method to store details of the external login in the store, so they can be used to sign into the application.
        // A redirection is performed that invokes the OnGetAsync method, which sets the property required to display the confirmation message to the user. For quick reference, Table 11-7 describes the sign-in methods used to create an account with an external login.
        public async Task<IActionResult> OnGetCallbackAsync() {
            ExternalLoginInfo info = await SignInManager.GetExternalLoginInfoAsync();
            string email = info?.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) {
                return Error("External service has not provided an email address.");
            } else if ((await UserManager.FindByEmailAsync(email)) != null ) {
                return Error("An account already exists with your email address.");
            }
            IdentityUser identUser = new IdentityUser {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            IdentityResult result = await UserManager.CreateAsync(identUser);
            if (result.Succeeded) {
                identUser = await UserManager.FindByEmailAsync(email);
                result = await UserManager.AddLoginAsync(identUser, info);
                return RedirectToPage(new { id = identUser.Id });
            }
            return Error("An account could not be created.");
        }
        public async Task<IActionResult> OnGetAsync(string id) {
            if (id == null) {
                return RedirectToPage("SignUp");
            } else {
                IdentityUser = await UserManager.FindByIdAsync(id);
                if (IdentityUser == null) {
                    return RedirectToPage("SignUp");
                }
            }
            return Page();
        }
        private IActionResult Error(string err) {
            TempData["errorMessage"] = err;
            return RedirectToPage();
        }
    }
}