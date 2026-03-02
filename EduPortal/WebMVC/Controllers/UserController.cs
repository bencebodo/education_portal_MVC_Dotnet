using EduPortal.Logic.Services.ProfileDetailsService;
using EduPortal.WebMVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPortal.WebMVC.Controllers
{
    [RequireUser]
    public class UserController : Controller
    {
        private readonly IProfileDetailsService _profileDetailsService;

        public UserController(IProfileDetailsService profileDetailsService)
        {
            _profileDetailsService = profileDetailsService ?? throw new ArgumentNullException(nameof(profileDetailsService));
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var userId = GetUserId();
            var profileDetailsData = await _profileDetailsService.GetProfileDetailsAsync(userId);
            return View(profileDetailsData);
        }
    }
}
