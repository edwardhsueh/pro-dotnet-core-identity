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
using System;

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
            /// The master list is managed using the role manager class, RoleManager<T> where T is the role class used by the application. In Chapter 7, I enabled support for roles by selecting the default role class, IdentityRole,
            /// adds the AddDefaultTokenProviders method to the chain of calls that set up Identity. This method sets up the services that are used to generate the confirmation/recovery tokens sent to users, which I describe in detail in Part 2.
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
                // enable password wrong lockup
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.AllowedForNewUsers = true;            
            }).AddEntityFrameworkStores<IdentityAppIdentityDbContext>()
            .AddDefaultTokenProviders();
            // A common requirement is to terminate any existing sessions when an account is lock out, which will prevent the user from using the application even if they had signed in before the lock started. 
            // The key to implementing this feature is the security stamp, which is a random string that is changed every time an alternation to the user’s security data is made. The first step is to configure Identity so that it periodically validates the cookies presented by the user to see if the security stamp has changed, as shown in Listing 9-21.
            // The validation feature is enabled by using the options pattern to assign an interval to the ValidationInterval property defined by the SecurityStampValidatorOptions class. I have chosen one minute for this example, but it is important to select an appropriate value for each project. Validation requires data to be retrieved from the user store, and if you set the interval too short, you will generate a large number of additional database queries, especially in applications with substantial concurrent users. On the other hand, setting the interval too long will extend the period a signed-in user will be able to continue using the application after their account is locked out.
            services.Configure<SecurityStampValidatorOptions>(opts => {
                opts.ValidationInterval = System.TimeSpan.FromMinutes(1);
            });            
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
            /// need to update the ASP.NET Core configuration. By default, ASP.NET Core will use the /Account/Login and /Account/Logout URLs for signing in and out of the application. 
            /// Could have used the routing system to ensure that my new Razor Pages will receive requests to these URLs, but I have chosen to change the URLs that ASP.NET Core uses instead
            /// LoginPath: This property is used to specify the URL to which the browser is directed following a challenge response so the user can sign into the application. for Example: authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication
            /// LogoutPath: This property is used to specify the URL to which the browser is directed so the user can sign into the application.
            /// AccessDeniedPath: This property is used to specify the URL to which the browser is directed following a forbidden response, indicating that the user does not have access to the requested content. For Example, autorization.
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
