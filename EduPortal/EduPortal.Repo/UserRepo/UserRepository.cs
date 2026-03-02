using EduPortal.Data.EPDbContext;
using EduPortal.Data.Models.Entites;
using EduPortal.Data.Models.Enum;
using EduPortal.Repo.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace EduPortal.Repo.UserRepo
{
    public class UserRepository(EduPortalDbContext context) : EFBaseRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetUserWithAllDetailsAsync(Guid userId)
        {
            return await GetDbContext().Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Materials)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesByUserIdAsync(Guid userId)
        {
            return await GetDbContext().Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Courses)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetUserSkillsByUserIdAsync(Guid userId)
        {
            return await GetDbContext().Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Skills)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCompletedCourseIdByUserIdsAsync(Guid userId)
        {
            return await GetDbContext().Users
                .Where(u => u.Id == userId && u.Courses.Any(uc => uc.IsCompleted))
                .SelectMany(u => u.Courses)
                .ToListAsync();
        }

        public async Task<HashSet<int>> GetCompletedMaterialIdsForCourseAsync(Guid userId, int courseId)
        {
            var query = from user in GetDbContext().Users
                        where user.Id == userId
                        from material in user.Materials
                        where material.IsCompleted
                              && material.Courses.Any(c => c.CourseId == courseId)
                        select material.MaterialId;

            var idList = await query.ToListAsync();
            return new HashSet<int>(idList);
        }

        public async Task MarkMaterialAsCompletedAsync(Guid userId, int materialId)
        {
            var user = await GetDbContext().Users
                .Include(u => u.Materials)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");

            var material = user.Materials.FirstOrDefault(m => m.MaterialId == materialId);
            if (material == null)
            {
                material = await GetDbContext().Materials.FindAsync(materialId)
                    ?? throw new KeyNotFoundException("Material not found");

                user.Materials.Add(material);
            }

            material.IsCompleted = true;

            await SaveAsync();
        }

        public async Task MarkCourseCompletedAsync(Guid userId, int courseId)
        {
            var user = await GetDbContext().Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");

            var course = user.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                course = await GetDbContext().Courses.FindAsync(courseId)
                    ?? throw new KeyNotFoundException("Course not found");

                user.Courses.Add(course);
            }

            course.IsCompleted = true;

            await SaveAsync();
        }

        public async Task<bool> IsUserEnrolledInCourseAsync(Guid userId, int courseId)
        {
            return await GetDbContext().Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Courses)
                .AnyAsync(c => c.CourseId == courseId);
        }

        public async Task MarkCourseEnrolledAsync(Guid userId, int courseId)
        {
            var user = await GetDbContext().Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");

            var course = await GetDbContext().Courses.FindAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found");

            if (!user.Courses.Any(c => c.CourseId == courseId))
            {
                user.Courses.Add(course);
                await SaveAsync();
            }
        }

        public async Task<Dictionary<int, int>> GetCompletedMaterialCountsByCourseAsync(Guid userId)
        {
            var result = await GetDbContext().Courses
                .Select(c => new
                {
                    c.CourseId,
                    CompletedCount = c.Materials.Count(m => m.Users.Any(u => u.Id == userId && u.Materials.Any(um => um.IsCompleted)))
                })
                .ToDictionaryAsync(x => x.CourseId, x => x.CompletedCount);

            return result;
        }

        public async Task AddOrUpgradeSkillsFromCourseAsync(Guid userId, int courseId)
        {
            var user = await GetDbContext().Users
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");

            var course = await GetDbContext().Courses
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.CourseId == courseId)
                ?? throw new KeyNotFoundException("Course not found");

            foreach (var courseSkill in course.Skills)
            {
                var userSkill = user.Skills.FirstOrDefault(us => us.SkillId == courseSkill.SkillId);

                if (userSkill == null)
                {
                    var trackedSkill = await GetDbContext().Skills
                        .FirstOrDefaultAsync(s => s.SkillId == courseSkill.SkillId);

                    if (trackedSkill != null && !user.Skills.Contains(trackedSkill))
                    {
                        trackedSkill.SkillLevel = SkillLevel.Beginner;
                        user.Skills.Add(trackedSkill);

                    }
                }
                else
                {
                    if (userSkill.SkillLevel < SkillLevel.Expert)
                    {
                        userSkill.SkillLevel++;
                    }
                }
            }

            await SaveAsync();
        }
    }
}