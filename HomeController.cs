using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestUserStore _users;
        private readonly IEventService _events;
        private readonly SignInManager<ExtendedIdentityUserModel> _signInManager;
        private readonly UserManager<ExtendedIdentityUserModel> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public HomeController(
            IEventService events,
            TestUserStore users,
            SignInManager<ExtendedIdentityUserModel> signInManager,
            UserManager<ExtendedIdentityUserModel> userManager,
            IIdentityServerInteractionService interactionService)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            _users = users;
            _events = events;
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
        }
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout(string logoutID)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutID);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Login", "Home");
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        [Authorize]
        public async Task<IActionResult> Welcome(LoginModel vm)
        {
            var users = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Message = users.FirstName + " " + users.LastName;

            return View(users);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Welcome", "Home");
                }
                ModelState.AddModelError("", "Invalid User Login Credentials....");
            }

            return View("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ExtendedIdentityUserModel(model.Username)
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Welcome", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditDetails(string userName)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var model = new EditUserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(EditUserModel mod)
        {
            ExtendedIdentityUserModel user = new ExtendedIdentityUserModel(mod.FirstName);

            //user.UserName = User.Identity.Name;
            user.FirstName = mod.FirstName;
            user.LastName = mod.LastName;
            user.Email = mod.Email;
            user.PasswordHash = mod.Password;
            user.SecurityStamp = Guid.NewGuid().ToString("D");

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Welcome", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Error Occurred while editing details...");
            }
            return View(mod);
        }

    }
}
