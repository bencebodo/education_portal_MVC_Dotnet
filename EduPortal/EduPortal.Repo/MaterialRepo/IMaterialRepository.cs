using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;

namespace EduPortal.Repo.MaterialRepo
{
    public interface IMaterialRepository : IRepository<Material>
    {
        Task<IEnumerable<Material>> SearchMaterialsAsync(string keyword);
    }
}
