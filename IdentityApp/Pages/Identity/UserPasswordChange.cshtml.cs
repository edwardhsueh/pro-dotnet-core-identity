using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
namespace IdentityApp.Pages.Identity {
    public class PasswordChangeBindingTarget {
        [Required]
        public string Current { get; set; }
        [Required]
        public string NewPassword{ get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword{ get; set; }
    }
    public class UserPasswordChangeModel : UserPageModel {
        public UserPasswordChangeModel(UserManager<IdentityUser> usrMgr)
            => UserManager = usrMgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        [BindProperty]
        public PasswordChangeBindingTarget PasswordChangeBindingTarget { get; set; }
        public async Task<IActionResult> OnPostAsync() {
            Console.WriteLine($"Current:{PasswordChangeBindingTarget.Current}, NewPasswd:{PasswordChangeBindingTarget.NewPassword}");
            if (ModelState.IsValid) {
                /// <summary>
                /// This method returns the IdentityUser object that has been stored for the specified ClaimsPrincipal object, which is most often obtained through the User property defined by the base classes for page models and controllers.
                /// </summary>
                /// <returns></returns>
                IdentityUser user = await UserManager.GetUserAsync(User);
                IdentityResult result = await UserManager.ChangePasswordAsync(user,PasswordChangeBindingTarget.Current, PasswordChangeBindingTarget.NewPassword);
                if (result.Process(ModelState)) {
                    TempData["message"] = "Password changed";
                    return RedirectToPage();
                }
            }
            return Page();
        }
    }
}