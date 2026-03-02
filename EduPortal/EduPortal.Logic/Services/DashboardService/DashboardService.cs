using EduPortal.Data.Models.Entites;
using EduPortal.Logic.DTOs.DashboardDTOs;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.UserRepo;

namespace EduPortal.Logic.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public DashboardService(IUserRepository userRepository, ICourseRepository courseRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }
        public async Task<DashboardDTO?> GetDashboardDataAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var enrolledCourses = await _userRepository.GetEnrolledCoursesByUserIdAsync(userId);
            var userSkills = await _userRepository.GetUserSkillsByUserIdAsync(userId);
            var allCourses = await _courseRepository.GetAllCoursesWithDetailsAsync();
            var completedMaterialCounts = await _userRepository.GetCompletedMaterialCountsByCourseAsync(userId);

            var enrolledCourseIds = enrolledCourses.Select(c => c.CourseId).ToHashSet();



            var courseListDTOs = allCourses
                .Select(course => MapToCourseListItemDTO(course, enrolledCourseIds, completedMaterialCounts))
                .ToList();

            var enrolledCourseSet = courseListDTOs.Where(c => c.IsEnrolled && c.ProgressPercentage < 100).ToList();
            var completedCourseSet = courseListDTOs.Where(c => c.IsEnrolled && c.ProgressPercentage >= 100).ToList();
            var availableCourseSet = courseListDTOs.Where(c => !c.IsEnrolled).ToList();

            var profileDto = new DashboardProfileDTO
            {
                FirstName = user.FirstName,
                CompletedCoursesCount = completedCourseSet.Count(),
                TotalSkillsCount = userSkills.Count()
            };

            return new DashboardDTO
            {
                DashboardProfileDTO = profileDto,
                EnrolledCourses = enrolledCourseSet,
                CompletedCourses = completedCourseSet,
                AvailableCourses = availableCourseSet
            };

        }

        private static CourseListItemDTO MapToCourseListItemDTO(
            Course course,
            HashSet<int> enrolledCourseIds,
            Dictionary<int, int> completedMaterialCounts)
        {
            bool isEnrolled = enrolledCourseIds.Contains(course.CourseId);
            completedMaterialCounts.TryGetValue(course.CourseId, out var completedCount);

            double progressPercentage = 0;
            if (isEnrolled)
            {
                progressPercentage = CalculateProgress(completedCount, course.Materials.Count);
            }
            return new CourseListItemDTO
            {
                CourseId = course.CourseId,
                CourseName = course.Name,
                CourseDescription = course.Description,
                CreatorName = course.Creator != null
                    ? $"{course.Creator.FirstName} {course.Creator.LastName}"
                    : "Unknown",
                MaterialCount = course.Materials.Count,
                SkillsCount = course.Skills.Count,
                IsEnrolled = isEnrolled,
                ProgressPercentage = progressPercentage
            };

        }

        public async Task MarkEnrolledCourseAsync(Guid userId, int courseId)
        {
            if (await _userRepository.IsUserEnrolledInCourseAsync(userId, courseId))
                return;

            await _userRepository.MarkCourseEnrolledAsync(userId, courseId);
        }
        private static double CalculateProgress(int completedCount, int totalCount)
        {
            if (totalCount == 0)
            {
                return 0;
            }
            return (double)completedCount / totalCount * 100;
        }
    }
}
