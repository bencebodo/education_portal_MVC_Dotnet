using EduPortal.Logic.DTOs.CourseDetailsDTOs;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Logic.Services.CourseDetailsService;
using EduPortal.WebMVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static EduPortal.Data.Models.Enum.MaterialType;

namespace EduPortal.WebMVC.Controllers
{
    [RequireUser]
    public class CourseController : Controller
    {
        private readonly ICourseDetailsService _courseDetailsService;
        private readonly ILogger<AccountController> _logger;
        public CourseController(ICourseDetailsService courseDetailsService, ILogger<AccountController> logger)
        {
            _courseDetailsService = courseDetailsService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var viewModelTask = await _courseDetailsService.GetCourseDetailsAsync(userId, id);

            return View(viewModelTask);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var userId = GetUserId();
            var viewModelTask = await _courseDetailsService.GetCourseDetailsAsync(userId, id);

            return View(viewModelTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseDetailsDTO dto)
        {
            var userId = GetUserId();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
                return View(dto);
            }

            if (!dto.CourseName.Any())
            {
                TempData["CourseError"] = "Please add Name to your Course.";
                return View(dto);
            }

            if (!dto.CourseDescription.Any())
            {
                TempData["CourseError"] = "Please add Description to your Course.";
                return View(dto);
            }

            if (!dto.Skills.Any())
            {
                TempData["CourseError"] = "Please add at least one skill to the course.";
                return View(dto);
            }

            if (!dto.Materials.Any())
            {
                TempData["CourseError"] = "Please add at least one material to the course.";
                return View(dto);
            }
            foreach (MaterialDTO materialDTO in dto.Materials)
            {
                if (materialDTO.MaterialType == Book && materialDTO.BookFile != null)
                {
                    var isSaveSuccess = await _courseDetailsService.SaveBookFileAsync(materialDTO);
                    if (!isSaveSuccess)
                    {
                        TempData["CourseError"] = "There was an error saving one of the book files. Please try again.";
                        return View(dto);
                    }
                }
            }
            var savedDto = await _courseDetailsService.AddOrUpdateCourseDetailsAsync(dto, userId);

            TempData["SuccessMessage"] = "Course saved successfully!";
            return RedirectToAction("Details", new { id = savedDto.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            CourseDetailsDTO dto;
            var userId = GetUserId();

            if (id.HasValue)
            {
                dto = await _courseDetailsService.GetCourseDetailsAsync(userId, id.Value);
                if (dto == null) return NotFound();
                if (userId != dto.CreatorId)
                {
                    TempData["ErrorMessage"] = "Only the creator can edit this course.";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                dto = new CourseDetailsDTO();
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(CourseDetailsDTO dto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid)
                return View(dto);
            var success = await _courseDetailsService.DeleteCourseDetailsAsync(dto, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Only the creator can delete this course.";
                return RedirectToAction("Index", "Dashboard");
            }

            TempData["SuccessMessage"] = "Course deleted successfully!";
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult GetEmptyMaterialPartial(int index)
        {
            var emptyMaterial = new MaterialDTO();
            ViewData["Index"] = index;
            return PartialView("_MaterialEditCardPartial", emptyMaterial);
        }

        [HttpGet]
        public async Task<IActionResult> BrowseSkills()
        {
            var allSkills = await _courseDetailsService.GetAllSkillDTOsAsync();
            return PartialView("_AddSkillTabPartial", allSkills);
        }

        [HttpGet]
        public async Task<IActionResult> BrowseMaterials()
        {
            var allMaterials = await _courseDetailsService.GetAllMaterialDTOsAsync();
            return PartialView("_AddMaterialTabPartial", allMaterials);
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialAsJson(int id)
        {
            var materialDtos = await _courseDetailsService.GetAllMaterialDTOsAsync();
            var materialDto = materialDtos.FirstOrDefault(m => m.MaterialId == id);

            if (materialDto == null) return NotFound();
            return Json(materialDto);
        }
    }
}
