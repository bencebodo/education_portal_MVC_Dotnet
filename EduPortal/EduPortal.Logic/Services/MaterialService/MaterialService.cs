using EduPortal.Data.Models.Entites;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.UserRepo;

namespace EduPortal.Logic.Services.MaterialService
{
    public class MaterialService : IMaterialService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public MaterialService(IUserRepository userRepository, ICourseRepository courseRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task<MaterialDTO?> GetMaterialDetailsAsync(Guid userId, int courseId, int materialId)
        {
            var course = await _courseRepository.GetCourseWithDetailsByIdAsync(courseId);
            if (course == null)
                return null;

            var material = course.Materials.FirstOrDefault(m => m.MaterialId == materialId);
            if (material == null)
                return null;

            var completedIds = await _userRepository.GetCompletedMaterialIdsForCourseAsync(userId, courseId) ?? new HashSet<int>();
            bool isCompleted = completedIds.Contains(materialId);

            return MapMaterialToDTO(material, isCompleted);

        }

        public async Task MarkMaterialAsCompletedAsync(Guid userId, int courseId, int materialId)
        {
            await _userRepository.MarkMaterialAsCompletedAsync(userId, materialId);

            await CheckAndMarkCourseCompletion(userId, courseId);
        }

        private static MaterialDTO MapMaterialToDTO(Material material, bool isCompleted)
        {
            var dto = new MaterialDTO
            {
                MaterialId = material.MaterialId,
                MaterialName = material.MaterialName,
                MaterialType = material.MaterialType,
                IsCompleted = isCompleted
            };

            switch (material)
            {
                case VideoMaterial video:
                    dto.DurationInSeconds = video.Duration.TotalSeconds;
                    dto.ResourceUrl = video.ResourceURL;
                    dto.Quality = video.Quality;
                    break;
                case BookMaterial book:
                    dto.BookTitle = book.Title;
                    dto.Author = book.Author;
                    dto.FilePath = book.FilePath;
                    dto.PublicationYear = book.PublicationYear;
                    dto.Format = book.Format;
                    dto.NumberOfPages = book.NumberOfPages;
                    break;
                case ArticleMaterial article:
                    dto.ResourceUrl = article.ResourceURL;
                    dto.PublicationYear = article.PublicationYear;
                    break;
            }

            return dto;
        }

        private async Task CheckAndMarkCourseCompletion(Guid userId, int courseId)
        {
            var course = await _courseRepository.GetCourseWithDetailsByIdAsync(courseId);
            if (course == null || course.Materials == null || !course.Materials.Any()) return;

            var completedIds = await _userRepository.GetCompletedMaterialIdsForCourseAsync(userId, courseId) ?? new HashSet<int>();

            if (completedIds.Count >= course.Materials.Count)
            {
                await _userRepository.MarkCourseCompletedAsync(userId, courseId);
                await _userRepository.AddOrUpgradeSkillsFromCourseAsync(userId, courseId);
            }
        }
    }
}