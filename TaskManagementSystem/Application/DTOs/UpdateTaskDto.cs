using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs
{
    public class UpdateTaskDto
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public TaskPriority? Priority { get; init; }
        public DateTime? Deadline { get; init; }
    }
}