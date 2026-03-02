using EduPortal.Data.Models.Enum;

namespace EduPortal.Data.Models.Entites
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public SkillLevel? SkillLevel { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
