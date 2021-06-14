using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace IdentityApp {

    public class Startup {

        public Startup(IConfiguration config) => Configuration = config;

        private IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<ProductDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:AppDataConnection"]);
            });

            services.AddHttpsRedirection(opts => {
                opts.HttpsPort = 44350;
            });
// Because the IdentityDbContext class is defined in a different assembly, I have to tell Entity Framework Core to create database migrations in the IdentityApp project, like this:            
            services.AddDbContext<IdentityDbContext>(opts => {
                            opts.UseSqlServer(
                                Configuration["ConnectionStrings:IdentityConnection"],
                                opts => opts.MigrationsAssembly("IdentityApp")
                            );
                        });
// The reason that ASP.NET Core threw exceptions for requests to restricted URLs in Chapter 3 was that no services had been registered to authentication requests. The AddDefaultIdentity method sets up those services using sensible default values. The generic type argument specifies the class Identity will use to represent users. The default class is IdentityUser, which is included in the Identity package .
// IdentityUser is known as the user class and is used by Identity to represent users. IdentityUser is the default user class provided by Microsoft. In Part 2, I create a custom user class, but IdentityUser is suitable for almost every project. The second part of this statement sets up the Identity datastore :            
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<IdentityDbContext>();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
