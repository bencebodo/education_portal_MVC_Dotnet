using EduPortal.Logic.Services.MaterialService;
using EduPortal.WebMVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPortal.WebMVC.Controllers
{
    [RequireUser]
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService ?? throw new ArgumentNullException(nameof(materialService));
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> View(int materialId, int courseId)
        {
            var userId = GetUserId();
            var viewModelTask = await _materialService.GetMaterialDetailsAsync(userId, courseId, materialId);

            if (viewModelTask == null)
            {
                return NotFound();
            }

            viewModelTask.CourseId = courseId;

            return View(viewModelTask);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsComplete(int materialId, int courseId)
        {
            var userId = GetUserId();
            await _materialService.MarkMaterialAsCompletedAsync(userId, courseId, materialId);

            return RedirectToAction("Index", "Course", new { id = courseId });
        }
    }
}
