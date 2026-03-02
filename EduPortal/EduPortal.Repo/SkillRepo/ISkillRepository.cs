using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;

namespace EduPortal.Repo.SkillRepo
{
    public interface ISkillRepository : IRepository<Skill>
    {
        Task<IEnumerable<Skill>> SearchSkillsAsync(string keyword);
    }
}
