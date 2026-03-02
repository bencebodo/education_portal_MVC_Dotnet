using EduPortal.Data.Models.Enum;
using EduPortal.Logic.DTOs.SharedDTO;
using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.ProfileDetailsDTO
{
    public class ProfileDetailDTO
    {
        public Guid UserId { get; set; }
        [DataType(DataType.Text)]
        public string? UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Text)]
        public string? FirstName { get; set; }
        [DataType(DataType.Text)]
        public string? LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public Dictionary<string, SkillLevel?> SkillsWithLevel { get; set; } = new Dictionary<string, SkillLevel?>();
        public List<CourseListItemDTO>? CoursesEnrolled { get; set; } = new List<CourseListItemDTO>();
        public List<CourseListItemDTO>? CompletedCourses { get; set; } = new List<CourseListItemDTO>();
    }
}
