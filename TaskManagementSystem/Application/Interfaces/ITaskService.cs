using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto);
        Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId);
        Task<IReadOnlyList<TaskDto>> GetByUserAsync(
            Guid userId,
            TaskState? state = null,
            TaskPriority? priority = null);

        Task<bool> MarkInProgressAsync(Guid taskId);
        Task<bool> CompleteAsync(Guid taskId);
    }
}
