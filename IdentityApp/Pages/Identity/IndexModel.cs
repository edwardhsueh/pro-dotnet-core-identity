using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity {
    public class IndexModel : UserPageModel {
        public IndexModel(UserManager<IdentityUser> userMgr)
            => UserManager = userMgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public async Task OnGetAsync() {
    // This method returns the IdentityUser object that has been stored for the specified ClaimsPrincipal object, which is most often obtained through the User property defined by the base classes for page models and controllers.
            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            Email = CurrentUser?.Email ?? "(No Value)";
            Phone = CurrentUser?.PhoneNumber ?? "(No Value)";
        }
    }
}