using EduPortal.Data.Models.Entites;
using EduPortal.Logic.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;


namespace EduPortal.Logic.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto, string role)
        {
            var user = MapDTOToEntity(dto);

            var existingUsers = await _userManager.FindByNameAsync(user.UserName);

            var existingEmail = await _userManager.FindByEmailAsync(user.Email);

            if (existingUsers != null)
            {
                var error = new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Username is already taken."
                };
                return IdentityResult.Failed(error);
            }

            if (existingEmail != null)
            {
                var error = new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email is already registered."
                };
                return IdentityResult.Failed(error);
            }

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return result;
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

                if (!roleCreationResult.Succeeded)
                {
                    return roleCreationResult;
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

            if (!addToRoleResult.Succeeded)
            {
                return addToRoleResult;
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                dto.UserName, dto.Password, dto.RememberMe, false);

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        private User MapDTOToEntity(RegisterDTO dto)
        {
            var user = new User();
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DateOfBirth = dto.DateOfBirth;

            return user;
        }
    }
}
