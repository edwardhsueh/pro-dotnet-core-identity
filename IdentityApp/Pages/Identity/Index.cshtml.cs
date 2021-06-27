using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
namespace IdentityApp.Pages.Identity {
    public class IndexModel : UserPageModel {
        public IndexModel(UserManager<IdentityUser> userMgr)
            => UserManager = userMgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    /// <summary>
    /// ASP.NET Core represents users with the ClaimsPrincipal class and a ClaimsPrincipal object for the current user is available through the User property defined by the Controller and RazorPageBase classes, which means the same features are available for the MVC Framework and Razor Pages
    /// This method returns the IdentityUser object that has been stored for the specified ClaimsPrincipal object, which is most often obtained through the User property defined by the base classes for page models and controllers.
    /// authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication, then opts.LoginPath of services.ConfigureApplicationCookie in startUp.cs will be used as a reDirect Path
    /// </summary>
    /// <returns></returns>
        
        public async Task OnGetAsync() {
            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            Console.WriteLine($"CurrentUser:{CurrentUser?.Email ?? "No Current User"}");
            Email = CurrentUser?.Email ?? "(No Value)";
            Phone = CurrentUser?.PhoneNumber ?? "(No Value)";
        }
    }
}