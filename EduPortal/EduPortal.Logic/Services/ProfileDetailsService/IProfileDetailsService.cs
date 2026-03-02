using EduPortal.Logic.DTOs.ProfileDetailsDTO;

namespace EduPortal.Logic.Services.ProfileDetailsService
{
    public interface IProfileDetailsService
    {
        Task<ProfileDetailDTO> GetProfileDetailsAsync(Guid userId);
    }
}
