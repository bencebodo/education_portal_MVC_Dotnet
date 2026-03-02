using EduPortal.Logic.DTOs.SharedDTO;

namespace EduPortal.Logic.Services.MaterialService
{
    public interface IMaterialService
    {
        Task<MaterialDTO?> GetMaterialDetailsAsync(Guid userId, int materialId, int courseId);
        Task MarkMaterialAsCompletedAsync(Guid userId, int courseId, int materialId);
    }
}
