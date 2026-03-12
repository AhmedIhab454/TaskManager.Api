using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class UpdateTaskDto
    {

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
