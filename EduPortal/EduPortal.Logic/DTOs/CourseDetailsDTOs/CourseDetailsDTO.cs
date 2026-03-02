using EduPortal.Logic.DTOs.SharedDTO;
using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.CourseDetailsDTOs
{
    public class CourseDetailsDTO
    {
        [Range(0, int.MaxValue)]
        public int CourseId { get; set; }
        [DataType(DataType.Text)]
        public string CourseName { get; set; } = string.Empty;
        [DataType(DataType.Text)]
        public string CourseDescription { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        [DataType(DataType.Text)]
        public string CreatorName { get; set; } = string.Empty;
        [Range(0, 100)]
        public double OverallProgressPercentage { get; set; }
        public List<MaterialDTO> Materials { get; set; } = new List<MaterialDTO>();
        public List<SkillDTO> Skills { get; set; } = new List<SkillDTO>();
        public List<string> AcquirableSkills { get; set; } = new List<string>();
    }
}
