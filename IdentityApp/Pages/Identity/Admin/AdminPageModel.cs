using Microsoft.AspNetCore.Authorization;
namespace IdentityApp.Pages.Identity.Admin {
    // The default policy of restricting access to signed-in users also applies to the administration dashboard, which will be a problem the next time the database is reset: the button that seeds the user store will be accessible only to signed-in users, but no sign ins are possible because the user store will be empty.

    // I’ll resolve this properly in Chapter 10 when I create workflows for managing roles. Until then I am going to allow anyone to access the administration features, even if there is no signed-in user. Add the attribute shown in Listing 8-32 to the AdminUserPage class.
    [AllowAnonymous]
    public class AdminPageModel: UserPageModel {
        // no methods or properties required
    }
}