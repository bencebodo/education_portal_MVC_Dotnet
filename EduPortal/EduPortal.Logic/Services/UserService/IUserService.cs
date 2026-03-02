using EduPortal.Logic.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;

namespace EduPortal.Logic.Services.UserService
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDTO dto, string role);
        Task<SignInResult> LoginAsync(LoginDTO dto);
        Task LogoutAsync();
    }
}
