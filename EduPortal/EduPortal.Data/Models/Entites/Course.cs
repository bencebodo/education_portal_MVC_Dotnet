namespace EduPortal.Data.Models.Entites
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public User Creator { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public Guid CreatorId { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<Material> Materials { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
