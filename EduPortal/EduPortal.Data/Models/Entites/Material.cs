using EduPortal.Data.Models.Enum;

namespace EduPortal.Data.Models.Entites
{
    public abstract class Material
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int MaterialOrder { get; set; }
        public bool IsCompleted { get; set; }
        public MaterialType MaterialType { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
