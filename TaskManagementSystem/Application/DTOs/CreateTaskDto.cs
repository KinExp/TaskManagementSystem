using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; init; } = null!;

        [MaxLength(1000)]
        public string? Description { get; init; }

        [Required]
        public TaskPriority Priority { get; init; } = TaskPriority.Medium;

        public DateTime? Deadline { get; init; }
    }
}
