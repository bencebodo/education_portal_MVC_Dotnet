using EduPortal.Data.EPDbContext;
using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace EduPortal.Repo.MaterialRepo
{
    public class MaterialRepository(EduPortalDbContext context) : EFBaseRepository<Material>(context), IMaterialRepository
    {
        public async Task<IEnumerable<Material>> SearchMaterialsAsync(string keyword)
        {
            var lowerKeyword = keyword.ToLower();
            return await GetDbContext().Materials
                .Where(m => m.MaterialName.ToLower().Contains(lowerKeyword))
                .ToListAsync();
        }
    }
}
