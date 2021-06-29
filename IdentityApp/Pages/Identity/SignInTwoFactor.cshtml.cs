using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System;
namespace IdentityApp.Pages.Identity {
    [AllowAnonymous]
    public class SignInTwoFactorModel : UserPageModel {
        public SignInTwoFactorModel(UserManager<IdentityUser> usrMgr,
                SignInManager<IdentityUser> signMgr) {
            UserManager = usrMgr;
            SignInManager = signMgr;
        }
        public UserManager<IdentityUser> UserManager { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }
        [BindProperty]
        [Required]
        public string Token { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }
        public async Task<IActionResult> OnPostAsync() {
            if (ModelState.IsValid) {
                //The sign-in manager’s GetTwoFactorAuthenticationUserAsync method retrieves the IdentityUser object associated with the email address and password provided in the previous step. If this method returns null, then the user has not provided a password, and the signing-in process should be stopped.
                IdentityUser user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
                if (user != null) {
                    string token = Regex.Replace(Token, @"\s", "");
                    Console.WriteLine($"Token:{token}");
                    // The sign-in manager’s TwoFactorAuthenticatorSignInAsync method is used to sign in with an authenticator token. The arguments are the token, a bool indicating whether the authentication cookie should be persistent, and a bool indicating whether a cookie should be created that will allow the user to sign in without the authenticator from the same browser.                    
                    SignInResult result = await
                         SignInManager.TwoFactorAuthenticatorSignInAsync(token, true,
                            RememberMe);
                    // If the user has provided a valid token, they are signed into the application. If the token is not valid, then I try and use it as a recovery code(user may input recovery code)
                    if (!result.Succeeded) {
                        result = await  SignInManager.TwoFactorRecoveryCodeSignInAsync(token);
                    }
                    if (result.Succeeded) {
                        // The user is signed in if the recovery code is valid. Codes can be used only once, and it is important to warn the user if they are running out of codes. I use the user manager’s CountRecoveryCodesAsync method to check how many are remaining and redirect the user to a warning page if there three or fewer codes left.
                        if (await UserManager.CountRecoveryCodesAsync(user) <= 3) {
                            return RedirectToPage("SignInCodesWarning");
                        }
                        return Redirect(ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Invalid token or recovery code");
            }
            return Page();
        }
    }
}
