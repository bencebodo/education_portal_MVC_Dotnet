using EduPortal.Logic.DTOs.IdentityDTOs;
using EduPortal.Logic.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace EduPortal.WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, string role)
        {
            if (!ModelState.IsValid)
                return View(registerDTO);

            var result = await _userService.RegisterUserAsync(registerDTO, role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    TempData["RegisterError"] = error.Description;
                }
                return View(registerDTO);
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");

            var result = await _userService.LoginAsync(loginDTO);

            if (result.Succeeded)
                return RedirectToAction("Index", "Dashboard");

            TempData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
