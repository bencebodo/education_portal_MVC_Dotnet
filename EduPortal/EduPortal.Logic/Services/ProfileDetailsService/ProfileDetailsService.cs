using EduPortal.Data.Models.Entites;
using EduPortal.Logic.DTOs.ProfileDetailsDTO;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Repo.UserRepo;

namespace EduPortal.Logic.Services.ProfileDetailsService
{
    public class ProfileDetailsService : IProfileDetailsService
    {
        private readonly IUserRepository _userRepository;

        public ProfileDetailsService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ProfileDetailDTO> GetProfileDetailsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserWithAllDetailsAsync(userId);

            var userSkills = user.Skills;

            var skillsWithLevel = userSkills
                .GroupBy(s => s.SkillName)
                .Select(g => g.First())
                .ToDictionary(s => s.SkillName, s => s.SkillLevel);

            var completedcourses = (await _userRepository.GetCompletedCourseIdByUserIdsAsync(userId));

            var enrolledCourses = await _userRepository.GetEnrolledCoursesByUserIdAsync(userId);

            var completedCoursesDto = enrolledCourses
                .Where(c => completedcourses.Any(cc => cc.CourseId == c.CourseId && cc.IsCompleted != false))
                .Select(MapCourseEntityToDTO)
                .ToList();

            var enrolledCoursesDto = enrolledCourses
                .Select(MapCourseEntityToDTO)
                .Where(c => !completedCoursesDto
                    .Any(cc => cc.CourseId == c.CourseId && cc.IsCompleted != true))
                .ToList();

            return new ProfileDetailDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                SkillsWithLevel = skillsWithLevel,
                CompletedCourses = completedCoursesDto,
                CoursesEnrolled = enrolledCoursesDto
            };
        }
        private static CourseListItemDTO MapCourseEntityToDTO(Course course)
        {
            return new CourseListItemDTO
            {
                CourseId = course.CourseId,
                CourseName = course.Name,
                CourseDescription = course.Description
            };
        }
    }
}