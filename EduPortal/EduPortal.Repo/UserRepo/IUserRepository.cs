using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;

namespace EduPortal.Repo.UserRepo
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithAllDetailsAsync(Guid userId);
        Task<IEnumerable<Course>> GetEnrolledCoursesByUserIdAsync(Guid userId);
        Task<IEnumerable<Skill>> GetUserSkillsByUserIdAsync(Guid userId);
        Task<IEnumerable<Course>> GetCompletedCourseIdByUserIdsAsync(Guid userId);
        Task<HashSet<int>> GetCompletedMaterialIdsForCourseAsync(Guid userId, int courseId);
        Task MarkMaterialAsCompletedAsync(Guid userId, int materialId);
        Task MarkCourseCompletedAsync(Guid userId, int courseId);
        Task<bool> IsUserEnrolledInCourseAsync(Guid userId, int courseId);
        Task MarkCourseEnrolledAsync(Guid userId, int courseId);
        Task<Dictionary<int, int>> GetCompletedMaterialCountsByCourseAsync(Guid userId);
        Task AddOrUpgradeSkillsFromCourseAsync(Guid userId, int courseId);
    }
}
