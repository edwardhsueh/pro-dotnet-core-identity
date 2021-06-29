using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System;
namespace IdentityApp.Pages.Identity {
    [AllowAnonymous]
    public class SignInModel : UserPageModel {
    
//         public SignInModel(SignInManager<IdentityUser> signMgr)
//             => SignInManager = signMgr;
        public SignInModel(SignInManager<IdentityUser> signMgr,
                UserManager<IdentityUser> usrMgr) {
            SignInManager = signMgr;
            // Handling Unconfirmed Sign-ins
            UserManager = usrMgr;
        }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public UserManager<IdentityUser>  UserManager { get; set; }
        [Required]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }
        [Required]
        [BindProperty]
        public string Password { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnPostAsync() {
            if (ModelState.IsValid) {
    // This method signs the user into the application with the specified username and password. The persist argument specifies whether the authentication cookie persists after the browser is closed. The lockout argument specifies whether a failed sign-in attempt counts toward a lockout, as described in Chapter 9. There is also a version of this method that accepts an IdentityUser object instead of a username.
                SignInResult result = await SignInManager.PasswordSignInAsync(Email,
                    Password, true, true);
                if (result.Succeeded) {
                    return Redirect(ReturnUrl ?? "/");
                } else if (result.IsLockedOut) {
                    TempData["message"] = "Account Locked";
                } else if (result.IsNotAllowed) {
                    IdentityUser user = await UserManager.FindByEmailAsync(Email);
                    if (user != null &&
                           !await UserManager.IsEmailConfirmedAsync(user)) {
                        return RedirectToPage("SignUpConfirm");
                    }
                    TempData["message"] = "Sign In Not Allowed";
                // When a user has set up an authenticator, the result of the PasswordSignInAsync method is a SignInResult object whose RequiresTwoFactor property is true 
                } else if (result.RequiresTwoFactor) {
                    return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
                } else {
                    TempData["message"] = "Sign In Failed";
                }
            }
            return Page();
        }
        // -----------------------------------------
        // for signin using external provider
        // -----------------------------------------
        public IActionResult OnPostExternalAsync(string provider) {
            string callbackUrl = Url.Page("SignIn", "Callback", new { ReturnUrl });
            Console.WriteLine($"callbackUrl:{callbackUrl}, ReturnUrl:{ReturnUrl}");
            AuthenticationProperties props = SignInManager.ConfigureExternalAuthenticationProperties(
                   provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }
        public async Task<IActionResult> OnGetCallbackAsync() {
            // This method returns a ExternalLoginInfo object that represents the user data provided by the external authentication service
            ExternalLoginInfo info = await SignInManager.GetExternalLoginInfoAsync();
            // This method signs the user into the application using a previously stored external login. The arguments are taken from the ExternalLoginInfo object returned by the GetExternalLoginInfoAsync method.
            SignInResult result = await SignInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded) {
                return Redirect(WebUtility.UrlDecode(ReturnUrl ?? "/"));
            } else if (result.IsLockedOut) {
                TempData["message"] = "Account Locked";
            } else if (result.IsNotAllowed) {
                TempData["message"] = "Sign In Not Allowed";
            } else {
                TempData["message"] = "Sign In Failed";
            }
            return RedirectToPage();
        }


    }
}