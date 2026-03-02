using EduPortal.Logic.DTOs.CourseDetailsDTOs;
using EduPortal.Logic.DTOs.SharedDTO;

namespace EduPortal.Logic.Services.CourseDetailsService
{
    public interface ICourseDetailsService
    {
        Task<CourseDetailsDTO> GetCourseDetailsAsync(Guid userId, int courseId);
        Task<CourseDetailsDTO> AddOrUpdateCourseDetailsAsync(CourseDetailsDTO courseDetailsDto, Guid userId);
        Task<bool> DeleteCourseDetailsAsync(CourseDetailsDTO dto, Guid creatorId);
        Task<List<SkillDTO>> GetAllSkillDTOsAsync();
        Task<List<MaterialDTO>> GetAllMaterialDTOsAsync();
        Task<bool> SaveBookFileAsync(MaterialDTO dto);
    }
}
