﻿using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

		public AccountController(UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
        {
			this.userManager = userManager;
            this.signInManager = signInManager;
		}

        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
				var identityUser = new IdentityUser
				{
					UserName = registerViewModel.Username,
					Email = registerViewModel.Email
				};

				var identityResult = await userManager.CreateAsync(identityUser, registerViewModel.Password);

				if (identityResult.Succeeded)
				{
					//asign this user the "User" role
					var roleIdentityResult = await userManager.AddToRoleAsync(identityUser, "User");

					if (roleIdentityResult.Succeeded)
					{
						// show success notification
						return RedirectToAction("Register");
					}
				}
			}
			//show error notification
			return View();
		}

        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel
            { 
                ReturnUrl = returnUrl 
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

           var signInResult =  await signInManager.PasswordSignInAsync(loginViewModel.Username, 
                loginViewModel.Password, false, false);

            if (signInResult != null && signInResult.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                {
                    return Redirect(loginViewModel.ReturnUrl);
                }


                return RedirectToAction("Index", "Home");
            }

            //show errors
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
