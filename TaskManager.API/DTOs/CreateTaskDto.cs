using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public string Priority { get; set; } = string.Empty;
    }
}
