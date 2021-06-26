using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authorization;
namespace IdentityApp.Pages.Identity {
    [AllowAnonymous]
    public class SignInModel : UserPageModel {
        public SignInModel(SignInManager<IdentityUser> signMgr)
            => SignInManager = signMgr;
        public SignInManager<IdentityUser> SignInManager { get; set; }
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
                    TempData["message"] = "Sign In Not Allowed";
                } else if (result.RequiresTwoFactor) {
                    return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
                } else {
                    TempData["message"] = "Sign In Failed";
                }
            }
            return Page();
        }
    }
}