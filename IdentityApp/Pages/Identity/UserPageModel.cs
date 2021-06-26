using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
namespace IdentityApp.Pages.Identity {
    // make authorization the default requirement and then explicitly grant the exceptions so that the intention for each page is obvious and consistent.
    [Authorize]
    public class UserPageModel : PageModel {
        // no methods or properties required
    }
}