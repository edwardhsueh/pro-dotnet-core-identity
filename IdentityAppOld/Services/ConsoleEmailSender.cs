
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Web;
namespace IdentityApp.Services {
    /// <summary>
    /// The Identity UI package includes an implementation of the interface whose SendEmailAsync method does nothing. I am going to create a dummy email service in this chapter because the process of setting up and integrating with a real service is beyond the scope of the book. In Chapter 17, where I describe the confirmation process in depth, I provide suggestions for commercial messaging platforms, but, in this book, I demonstrate the Identity support for confirmations by writing messages to the .NET console. Create the IdentityApp/Services folder and add to it a class file named ConsoleEmailSender.cs with the code shown in Listing 4-13.
    /// </summary>
    public class ConsoleEmailSender : IEmailSender {
        public Task SendEmailAsync(string emailAddress,
                string subject, string htmlMessage) {
            System.Console.WriteLine("---New Email----");
            System.Console.WriteLine($"To: {emailAddress}");
            System.Console.WriteLine($"Subject: {subject}");
            System.Console.WriteLine(HttpUtility.HtmlDecode(htmlMessage));
            System.Console.WriteLine("-------");
            return Task.CompletedTask;
        }
    }
}
