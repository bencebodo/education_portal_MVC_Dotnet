using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;

namespace EduPortal.Repo.CourseRepo
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetCoursesByCreatorIdAsync(Guid creatorId);
        Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync();
        Task<Course?> GetCourseWithDetailsByIdAsync(int courseId);
        Task<IEnumerable<Course>> GetCoursesBySkillAsync(int skillId);
        Task<IEnumerable<Course>> SearchCoursesAsync(string keyword);
        Task<Course> AddOrUpdateCourseAsync(Course course);
    }
}
