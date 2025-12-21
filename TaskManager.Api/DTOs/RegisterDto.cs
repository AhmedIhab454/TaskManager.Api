using System.ComponentModel.DataAnnotations;
namespace TaskManager.Api.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string UsreName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
