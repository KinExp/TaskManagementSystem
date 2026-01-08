using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }

        public TaskState State { get; init; }
        public TaskPriority Priority { get; init; }

        public DateTime? Deadline { get; init; }
    }
}
