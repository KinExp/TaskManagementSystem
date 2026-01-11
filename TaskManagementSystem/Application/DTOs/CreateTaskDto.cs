using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public TaskPriority Priority { get; init; } = TaskPriority.Medium;
        public DateTime? Deadline { get; init; }
    }
}
