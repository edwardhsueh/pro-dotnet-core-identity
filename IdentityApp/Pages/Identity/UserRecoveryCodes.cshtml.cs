using Microsoft.AspNetCore.Mvc;
namespace IdentityApp.Pages.Identity {
    public class UserRecoveryCodesModel : UserPageModel {
    /// <summary>
    /// The TempData attribute is used to set the value of the RecoveryCodes property
    /// </summary>
    /// <value></value>
        [TempData]
        public string[] RecoveryCodes { get; set; }
        public IActionResult OnGet() {
            if (RecoveryCodes == null || RecoveryCodes.Length == 0) {
                return RedirectToPage("UserTwoFactorManage");
            }
            return Page();
        }
    }
}