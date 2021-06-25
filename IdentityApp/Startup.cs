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
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityApp.Services;
using IdentityApp.Areas.Identity.Data;

namespace IdentityApp {

    public class Startup {

        public Startup(IConfiguration config) => Configuration = config;

        private IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            //Use HttpContext from custom components
            //IdentityEmailService
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddDbContext<ProductDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:AppDataConnection"]);
            });

            services.AddHttpsRedirection(opts => {
                opts.HttpsPort = 44350;
            });
            //    
            // for Identity
            //
            services.AddDbContext<IdentityAppIdentityDbContext>(options => { 
                options.UseSqlServer(
                   Configuration["ConnectionStrings:IdentityAppIdentityDbContextConnection"]);
            });       
            // have registered the email service before the call to the AddDefaultIdentity method so that my custom service takes precedence over the placeholder implementation in the Identity UI package.
            services.AddScoped<IEmailSender, ConsoleEmailSender>();
            /// <summary>
            /// The user class is declared when configuring Identity in the Startup class. Here is the statement that sets up Identity in the example application: There is a default class, named IdentityUser
            /// The AddEntityFrameworkStores method sets up the user store, and the generic type argument specifies the Entity Framework Core context that will be used to access the database.
            /// Caution is required because not all user stores support all of the Identity features, which is indicated by the set of ///  optional interfaces that the user store has implemented. It is important to check that all the features you require are supported when you start a new project and when you change user store.    
            /// </summary>
            /// <value></value>
            // services.AddDefaultIdentity<IdentityUser>(opts => { 
            //     opts.SignIn.RequireConfirmedAccount = true;
            //     opts.Password.RequiredLength = 8;
            //     opts.Password.RequireDigit = false;
            //     opts.Password.RequireLowercase = false;
            //     opts.Password.RequireUppercase = false;
            //     opts.Password.RequireNonAlphanumeric = false;
            //     opts.SignIn.RequireConfirmedAccount = true;        
            // }).AddEntityFrameworkStores<IdentityAppIdentityDbContext>();

            /// <summary>
            /// I have replaced the AddDefaultIdentity method the AddIdentity method. The AddIdentity method defines an additional generic type parameter that is used to specify the role class, which enables role support in the user store.
            ///  IdentityRole = > Support Role: The user store set up by the AddEntityFrameworkStores method does support roles but only when a role class has been selected, which isn’t possible with the AddDefaultIdentity method used previously.
            /// adds the AddDefaultTokenProviders method to the chain of calls that set up Identity. This method sets up the services that are used to generate the confirmation tokens sent to users, which I describe in detail in Part 2.
            /// ASP.NET Core represents users with the ClaimsPrincipal class and a ClaimsPrincipal object for the current user is available through the User property defined by the Controller and RazorPageBase classes, which means the same features are available for the MVC Framework and Razor Pages
            /// </summary>
            /// <value></value>    
            services.AddIdentity<IdentityUser, IdentityRole>(opts => { 
                opts.SignIn.RequireConfirmedAccount = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.SignIn.RequireConfirmedAccount = true;        
            }).AddEntityFrameworkStores<IdentityAppIdentityDbContext>()
            .AddDefaultTokenProviders();;
            services.AddScoped<TokenUrlEncoderService>();
            services.AddScoped<IdentityEmailService>();

            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        IConfigurationSection googleAuthNSection =
                            Configuration.GetSection("Authentication:Google");

                        options.ClientId = googleAuthNSection["ClientId"];
                        options.ClientSecret = googleAuthNSection["ClientSecret"];
                    })
                    .AddFacebook(facebookOptions =>
                    {
                        facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                        facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    });                    
            ;            
            /// <summary>
            /// need to update the ASP.NET Core configuration. By default, ASP.NET Core will use the /Account/Login and /Account/Logout URLs for signing in and out of the application. I could have used the routing system to ensure that my new Razor Pages will receive requests to these URLs, but I have chosen to change the URLs that ASP.NET Core uses instead
            /// LoginPath: This property is used to specify the URL to which the browser is directed following a challenge response so the user can sign into the application.
            /// LogoutPath: This property is used to specify the URL to which the browser is directed so the user can sign into the application.
            /// AccessDeniedPath: This property is used to specify the URL to which the browser is directed following a forbidden response, indicating that the user does not have access to the requested content.   
            /// </summary>
            /// <value></value>
            services.ConfigureApplicationCookie(opts => {
                opts.LoginPath = "/Identity/SignIn";
                opts.LogoutPath = "/Identity/SignOut";
                opts.AccessDeniedPath = "/Identity/Forbidden";
            });

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
