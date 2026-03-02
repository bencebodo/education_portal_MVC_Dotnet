using EduPortal.Logic.Services.DashboardService;
using EduPortal.WebMVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPortal.WebMVC.Controllers
{
    [RequireUser]
    public class DashboardController : Controller
    {

        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        }

        private Guid GetUserId()
        {

            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var userId = GetUserId();
            var dashboardData = await _dashboardService.GetDashboardDataAsync(userId);

            if (dashboardData == null)
            {
                return View("Index", "Home");
            }

            return View(dashboardData);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            var userId = GetUserId();
            try
            {
                await _dashboardService.MarkEnrolledCourseAsync(userId, courseId);
                return RedirectToAction("Index", "Course", new { id = courseId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Course not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
