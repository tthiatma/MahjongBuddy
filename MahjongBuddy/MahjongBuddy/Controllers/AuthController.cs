using MahjongBuddy.Models;
using MahjongBuddy.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MahjongBuddy.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {

        private readonly UserManager<AppUser> userManager;

        public AuthController()
            : this(Startup.UserManagerFactory.Invoke())
        {         
        }

        public AuthController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new LogInModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindAsync(model.Email, model.Password);

            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ApplicationCookie);

                GetAuthenticationManager().SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            ModelState.AddModelError("", "Invalid Email or Password");

            return View();        
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return View();                
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Country = model.Country
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignIn(user);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("index", "home");
        }

        private async Task SignIn(AppUser user)
        {
            var identity = await userManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);

            GetAuthenticationManager().SignIn(identity);
        }
        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }
    }
}
