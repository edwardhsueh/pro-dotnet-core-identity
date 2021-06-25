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
    /// <summary>
    /// ASP.NET Core represents users with the ClaimsPrincipal class and a ClaimsPrincipal object for the current user is available through the User property defined by the Controller and RazorPageBase classes, which means the same features are available for the MVC Framework and Razor Pages
    /// This method returns the IdentityUser object that has been stored for the specified ClaimsPrincipal object, which is most often obtained through the User property defined by the base classes for page models and controllers.
    /// </summary>
    /// <returns></returns>
            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            Email = CurrentUser?.Email ?? "(No Value)";
            Phone = CurrentUser?.PhoneNumber ?? "(No Value)";
        }
    }
}