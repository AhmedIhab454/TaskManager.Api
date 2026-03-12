using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Usrename { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
