using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.SharedDTO
{
    public class SkillDTO
    {
        [Range(0, int.MaxValue)]
        public int SkillId { get; set; }
        [DataType(DataType.Text)]
        public string SkillName { get; set; }
    }
}
