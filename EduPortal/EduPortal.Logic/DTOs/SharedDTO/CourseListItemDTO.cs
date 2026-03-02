using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.SharedDTO
{
    public class CourseListItemDTO
    {
        [Range(0, int.MaxValue)]
        public int CourseId { get; set; }
        [DataType(DataType.Text)]
        public string? CourseName { get; set; }
        [DataType(DataType.Text)]
        public string? CourseDescription { get; set; }
        [DataType(DataType.Text)]
        public string? CreatorName { get; set; }
        [DataType(DataType.Text)]
        public int SkillsCount { get; set; }
        [Range(1, 12)]
        public int MaterialCount { get; set; }
        public bool IsEnrolled { get; set; }
        [Range(0, 100)]
        public double ProgressPercentage { get; set; }
        public bool IsCompleted => IsEnrolled && ProgressPercentage == 100;
    }
}
