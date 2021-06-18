using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity.Admin {
    public class ViewModel : AdminPageModel {

        // dependency injection
        public ViewModel(UserManager<IdentityUser> mgr) => UserManager = mgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public IdentityUser IdentityUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public IEnumerable<string> PropertyNames
            => typeof(IdentityUser).GetProperties()
                .Select(prop => prop.Name);
        public string GetValue(string name) =>
            typeof(IdentityUser).GetProperty(name)
                 .GetValue(IdentityUser)?.ToString();
        public async Task<IActionResult> OnGetAsync() {
            if (string.IsNullOrEmpty(Id)) {
                return RedirectToPage("Selectuser",
                    new { Label = "View User", Callback = "View" });
            }
// This method returns an IdentityUser object representing the user with the specified unique ID.
            IdentityUser = await UserManager.FindByIdAsync(Id);
            return Page();
        }
    }
}