using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace IdentityApp.Pages.Identity.Admin {
    public class LockoutsModel : AdminPageModel {
        public LockoutsModel(UserManager<IdentityUser> usrMgr)
            => UserManager = usrMgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public IEnumerable<IdentityUser> LockedOutUsers { get; set; }
        public IEnumerable<IdentityUser> OtherUsers { get; set; }
        public async Task<TimeSpan> TimeLeft(IdentityUser user)
            => (await UserManager.GetLockoutEndDateAsync(user))
                .GetValueOrDefault().Subtract(DateTimeOffset.Now);
        public void OnGet() {
            Console.WriteLine($"DateTimeOffset.Now:{DateTimeOffset.Now.ToString()}");
            /// <summary>
            /// To determine if a user is locked out, the user manager’s GetLockoutEndDateAsync method is called. If the result has a value and that value specifies a time in the future, then the user is locked out. If there is no value or the time has passed, then the user is not locked out.
            /// </summary>
            /// <returns></returns>
            LockedOutUsers = UserManager.Users.Where(user => user.LockoutEnd.HasValue
                    && user.LockoutEnd.Value > DateTimeOffset.Now)
                .OrderBy(user => user.Email).ToList();
            OtherUsers = UserManager.Users.Where(user => !user.LockoutEnd.HasValue
                    || user.LockoutEnd.Value <= DateTimeOffset.Now)
                 .OrderBy(user => user.Email).ToList();
        }
        public async Task<IActionResult> OnPostLockAsync(string id) {
            IdentityUser user = await UserManager.FindByIdAsync(id);
/// <summary>
/// To lock and unlock the user account, the SetLockoutEnabledAsync method is used to enable lockouts, and a time is specified using the SetLockoutEndDateAsync method. The user won’t be allowed to sign in until the specified time has passed. Lockouts can be disabled by calling the SetLockoutEndDateAsync with a null argument.
/// </summary>
/// <returns></returns>
            await UserManager.SetLockoutEnabledAsync(user, true);
            await UserManager.SetLockoutEndDateAsync(user,
                DateTimeOffset.Now.AddDays(5));
            // change the security stamp when locking out an accoun
            // user will be signed out according to SecurityStampValidatorOptions set in startup.cs
            await UserManager.UpdateSecurityStampAsync(user);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUnlockAsync(string id) {
            IdentityUser user = await UserManager.FindByIdAsync(id);
            await UserManager.SetLockoutEndDateAsync(user, null);
            return RedirectToPage();
        }
    }
}