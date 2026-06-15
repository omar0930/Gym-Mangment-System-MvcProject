using GymMangmentSystem.BLL.ViewModels.AccountViewModels;
using GymMangmentSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymMangmentSystem.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly ILogger<AccountController> _logger;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _signInManger = signInManager;
            _userManger = userManager;
            _logger = logger;
        }
        //login
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user is null || string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            var result = await _signInManger.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} signed in.", user.Id);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} account is locked out.", user.Id);
                ModelState.AddModelError(string.Empty, "User account is locked out.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "User is not allowed to sign in.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }


        //signout
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManger.SignOutAsync();
            _logger.LogInformation("User signed out.");
            return RedirectToAction(nameof(login));
        }


        //accessDenied
        public IActionResult AccessDenied() => View();
    }

}
