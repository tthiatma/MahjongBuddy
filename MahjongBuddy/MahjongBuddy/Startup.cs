using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartup(typeof(MahjongBuddy.Startup))]
namespace MahjongBuddy
{
    public class Startup
    {
        public static Func<UserManager<AppUser>> UserManagerFactory { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            GlobalHost.HubPipeline.AddModule(new LoggingPipeLineModule());

            app.UseCookieAuthentication(new CookieAuthenticationOptions 
            { 
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });

            UserManagerFactory = () =>
                {
                    var userManager = new UserManager<AppUser>(new UserStore<AppUser>(new AppDbContext()));
                
                    userManager.UserValidator = new UserValidator<AppUser>(userManager)
                    {
                        AllowOnlyAlphanumericUserNames = false
                    };

                    userManager.ClaimsIdentityFactory = new AppUserClaimsIdentityFactory();

                    return userManager;
                };

            app.MapSignalR();
        }
    }
}
