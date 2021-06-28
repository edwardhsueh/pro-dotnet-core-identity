using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
namespace IdentityApp {
    /// <summary>
    /// The class defines an extension method that makes it easy to prepare the user store from the Startup class. A new dependency injection scope is created, and the role and user managers are used to ensure the required role and user exist. The name of the role, the name of the user, and the user’s initial password are obtained from the ASP.NET Core configuration system so that the default values are easily overridden
    /// Populating the User and Role Store During Startup
    /// </summary>
    public static class DashBoardSeed {
        public static void SeedUserStoreForDashboard(this IApplicationBuilder app) {
            SeedStore(app).GetAwaiter().GetResult();
        }
        private async static Task SeedStore(IApplicationBuilder app) {
            using (var scope = app.ApplicationServices.CreateScope()) {
                IConfiguration config = scope.ServiceProvider.GetService<IConfiguration>();
                UserManager<IdentityUser> userManager =
                    scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                RoleManager<IdentityRole> roleManager =
                    scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                string roleName = config["Dashboard:Role"] ?? "Dashboard";
                string userName = config["Dashboard:User"] ?? "admin@example.com";
                string password = config["Dashboard:Password"] ?? "mypasswd";
                if (!await roleManager.RoleExistsAsync(roleName)) {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                IdentityUser dashboardUser = await userManager.FindByEmailAsync(userName);
                if (dashboardUser == null) {
                    dashboardUser = new IdentityUser {
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(dashboardUser);
                    dashboardUser = await userManager.FindByEmailAsync(userName);
                    await userManager.AddPasswordAsync(dashboardUser, password);
                }
                if (!await userManager.IsInRoleAsync(dashboardUser, roleName)) {
                    await userManager.AddToRoleAsync(dashboardUser, roleName);
                }
            }
        }
    }
}