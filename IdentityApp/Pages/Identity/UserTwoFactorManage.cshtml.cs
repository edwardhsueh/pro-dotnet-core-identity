using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity {
    public class UserTwoFactorManageModel : UserPageModel {
        public UserTwoFactorManageModel(UserManager<IdentityUser> usrMgr,
                SignInManager<IdentityUser> signMgr) {
            UserManager = usrMgr;
            SignInManager = signMgr;
        }
        public UserManager<IdentityUser> UserManager { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IdentityUser IdentityUser { get; set; }
        /// <summary>
        /// The user manager’s GetTwoFactorEnabledAsync method is used to determine if the user has configured an authenticator. Applications can support multiple forms of two-factor authentication, as I demonstrate in Part 2, but this example uses only authenticators, so I can assume that an account with two-factor authentication enabled has been configured with an authenticator.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsTwoFactorEnabled()
            => await UserManager.GetTwoFactorEnabledAsync(IdentityUser);
        public async Task OnGetAsync() {
            IdentityUser = await UserManager.GetUserAsync(User);
        }
        /// <summary>
        /// The SetTwoFactorEnabledAsync method is used to enable and disable two-factor authentication. In this class, I only need to disable two-factor authentication in the Disable POST handler method.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDisable() {
            IdentityUser = await UserManager.GetUserAsync(User);
            IdentityResult result = await
                UserManager.SetTwoFactorEnabledAsync(IdentityUser, false);
            if (result.Process(ModelState)) {
                await SignInManager.SignOutAsync();
                return RedirectToPage("Index", new { });
            }
            return Page();
        }
        /// <summary>
        /// The GenerateCodes POST handler method generates a new set of recovery codes. These codes are shown to the user only once and are consumed when they are used, so it is important to ensure that the user can create a new set, either when they use up the codes or forget them
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostGenerateCodes() {
            IdentityUser = await UserManager.GetUserAsync(User);
            TempData["RecoveryCodes"] =
                await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(
                    IdentityUser, 10);
            return RedirectToPage("UserRecoveryCodes");
        }
    }
}