using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity {
    public class SignOutModel : UserPageModel    {
        public SignOutModel(SignInManager<IdentityUser> signMgr)
            => SignInManager = signMgr;
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public async Task<IActionResult> OnPostAsync() {
    // This method signs the current user out of the application
            await SignInManager.SignOutAsync();
            return RedirectToPage();
        }
    }
}