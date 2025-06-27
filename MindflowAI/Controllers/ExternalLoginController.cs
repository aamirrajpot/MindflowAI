using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindflowAI.Entities.AppUser;
using MindflowAI.Services.Dtos.AppUser;
using MindflowAI.Services.User;
using System.Security.Claims;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace MindflowAI.Controllers
{
    [Route("external-login")]
    public class ExternalLoginController : AbpController
    {
        private readonly UserAccountAppService _userAccountAppService;
        private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
        private readonly IdentityUserManager _userManager;
        public ExternalLoginController(UserAccountAppService userAccountAppService, SignInManager<Volo.Abp.Identity.IdentityUser> signInManager, IdentityUserManager userManager)
        {
            _userAccountAppService = userAccountAppService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "ExternalLogin");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google";
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User";

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Register new user with a random password
                var password = Guid.NewGuid().ToString("N") + "!Aa1"; // Strong random password
                var registerDto = new RegisterWithOtpDto
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password,
                    ConfirmPassword = password
                };
                await _userAccountAppService.RegisterAsync(registerDto);
                user = await _userManager.FindByEmailAsync(email);
            }

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
    }
}
