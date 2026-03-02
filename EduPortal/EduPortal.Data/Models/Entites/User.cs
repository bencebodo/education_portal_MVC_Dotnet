using Microsoft.AspNetCore.Identity;

namespace EduPortal.Data.Models.Entites
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Material> Materials { get; set; }
    }
}
