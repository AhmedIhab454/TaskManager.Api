using System.ComponentModel.DataAnnotations;
namespace TaskManager.Api.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Usrename { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]

        public string Password { get; set; } = null!;
    }
}
