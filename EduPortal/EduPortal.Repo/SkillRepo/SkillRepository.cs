using EduPortal.Data.EPDbContext;
using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace EduPortal.Repo.SkillRepo
{
    public class SkillRepository(EduPortalDbContext context) : EFBaseRepository<Skill>(context), ISkillRepository
    {
        public async Task<IEnumerable<Skill>> SearchSkillsAsync(string keyword)
        {
            var lowerKeyword = keyword.ToLower();
            return await GetDbContext().Skills
                .Where(s => s.SkillName.ToLower().Contains(lowerKeyword))
                .ToListAsync();
        }
    }
}
