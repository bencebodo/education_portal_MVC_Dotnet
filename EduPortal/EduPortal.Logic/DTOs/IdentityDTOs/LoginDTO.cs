using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.IdentityDTOs
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
