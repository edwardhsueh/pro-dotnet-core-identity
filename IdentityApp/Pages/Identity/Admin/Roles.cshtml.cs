using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace IdentityApp.Pages.Identity.Admin {
    public class RolesModel : AdminPageModel {
        public RolesModel(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleMgr,
                          IConfiguration config      
        ) {
            UserManager = userMgr;
            RoleManager = roleMgr;
            DashboardRole = config["Dashboard:Role"] ?? "Dashboard";
        }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public string DashboardRole { get; }
        public UserManager<IdentityUser> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }
        public IList<string> CurrentRoles { get; set; } = new List<string>();
        public IList<string> AvailableRoles { get; set; } = new List<string>();
/// <summary>
/// RoleManager.Roles: This property returns an IQueryable<IdentityRole> that allows the roles in the role store to be enumerated or queried with LINQ.
/// UserManager.GetRolesAsync(user): This method returns an IList<string> containing the names of all the roles to which the user has been assigned.
/// </summary>
/// <returns></returns>
        private async Task SetProperties() {
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            CurrentRoles = await UserManager.GetRolesAsync(user);
            AvailableRoles = RoleManager.Roles.Select(r => r.Name)
                .Where(r => !CurrentRoles.Contains(r)).ToList();
        }
        public async Task<IActionResult> OnGetAsync() {
            if (string.IsNullOrEmpty(Id)) {
                return RedirectToPage("Selectuser",
                    new { Label = "Edit Roles", Callback = "Roles" });
            }
            await SetProperties();
            return Page();
        }
        /// <summary>
        /// CreateAsync: This method adds the specified IdentityRole object to the role store and returns an IdentityResult object that describes the outcome.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAddToList(string role) {
            IdentityResult result =
                await RoleManager.CreateAsync(new IdentityRole(role));
            if (result.Process(ModelState)) {
                return RedirectToPage();
            }
            await SetProperties();
            return Page();
        }
        /// <summary>
        /// DeleteAsync: This method removes the specified IdentityRole object from the role store and returns an IdentityResult object that describes the outcome.
        /// FindByNameAsync: This method locates the IdentityRole object with the specified name in the role store.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteFromList(string role) {
            IdentityRole idRole = await RoleManager.FindByNameAsync(role);
            IdentityResult result = await RoleManager.DeleteAsync(idRole);
            if (result.Process(ModelState)) {
                return RedirectToPage();
            }
            await SetProperties();
            return Page();
        }
        
        /// <summary>
        /// IsInRoleAsync: This method returns true if the user has been assigned to the role with the specified name and false otherwise.
        /// AddToRoleAsync: This method assigns the user to the role with the specified name and returns an IdentityResult object that describes the outcome. There is also an AddToRolesAsync method that assigns the user to multiple roles at once.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAdd([Required] string role) {
            if (ModelState.IsValid) {
                IdentityResult result = IdentityResult.Success;
                if (result.Process(ModelState)) {
                    IdentityUser user = await UserManager.FindByIdAsync(Id);
                    if (!await UserManager.IsInRoleAsync(user, role)) {
                        result = await UserManager.AddToRoleAsync(user, role);
                    }
                    if (result.Process(ModelState)) {
                        return RedirectToPage();
                    }
                }
            }
            await SetProperties();
            return Page();
        }

        /// <summary>
        /// RemoveFromRoleAsync: This method removes the user from the role with the specified name and returns an IdentityResult object that describes the outcome. There is also a RemoveFromRolesAsync method that removes the user from multiple roles at once.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDelete(string role) {
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            if (await UserManager.IsInRoleAsync(user, role)) {
                await UserManager.RemoveFromRoleAsync(user, role);
            }
            return RedirectToPage();
        }
    }
}