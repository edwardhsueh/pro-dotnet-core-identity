using System;
using IdentityApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityApp.Services;
[assembly: HostingStartup(typeof(IdentityApp.Areas.Identity.IdentityHostingStartup))]
namespace IdentityApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityDbContextIdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDbContextIdentityDbContextConnection")));
            /// <summary>
            /// registered the ConsoleEmailSender class as the implementation of the IEmailSender that will be used for dependency injection.
            /// </summary>
            /// <typeparam name="IEmailSender"></typeparam>
            /// <typeparam name="ConsoleEmailSender"></typeparam>
            /// <returns></returns>
            services.AddScoped<IEmailSender, ConsoleEmailSender>();

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<IdentityDbContextIdentityDbContext>();
            });
        }
    }
}