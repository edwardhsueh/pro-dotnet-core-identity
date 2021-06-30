using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
namespace IdentityApp.Pages.Identity {
    public static class IdentityExtensions {
        /// <summary>
        /// Each IdentityError object in the sequence returned by the IdentityResult.Errors property defines a Code property and a Description property. The Code property is used to unambiguously identify the error and is intended to be consumed by the application. I am interested in the Description property, which describes an error that can be presented to the user. I use the foreach keyword to add the value from each IdentityError.Description property and add it to the set of validation errors that ASP.NET Core will handle.    
        /// </summary>
        /// <param name="result"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static bool Process(this IdentityResult result, ModelStateDictionary modelState) {
            foreach (IdentityError err in result.Errors
                    ?? Enumerable.Empty<IdentityError>()) {
                modelState.AddModelError(string.Empty, err.Description);
            }
            return result.Succeeded;
        }
    }
}