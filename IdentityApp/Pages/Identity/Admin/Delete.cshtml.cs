using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace IdentityApp.Pages.Identity.Admin {
    public class DeleteModel : AdminPageModel {
        public DeleteModel(UserManager<IdentityUser> mgr) => UserManager = mgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public IdentityUser IdentityUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public async Task<IActionResult> OnGetAsync() {
            if (string.IsNullOrEmpty(Id)) {
                return RedirectToPage("Selectuser",
                    new { Label = "Delete", Callback = "Delete" });
            }
            IdentityUser = await UserManager.FindByIdAsync(Id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync() {
            IdentityUser = await UserManager.FindByIdAsync(Id);
            /// <summary>
            /// This method removes the specified IdentityUser object from the user store.
            /// Notice that I don’t need to update the security stamp to force an immediate sign-out in Listing 9-25 because the IdentityUser data—including the security stamp—is removed from the user store. If cookie validation is enabled, as shown in Listing 9-21, the user will be automatically signed out the next time validation is performed.
            /// </summary>
            /// <returns></returns>
            IdentityResult result = await UserManager.DeleteAsync(IdentityUser);
            if (result.Process(ModelState)) {
                return RedirectToPage("Dashboard");
            }
            return Page();
        }
    }
}