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

        Task<bool> UpdateAsync(
            Guid userId,
            Guid taskId,
            string? title,
            string? description,
            TaskPriority? priority,
            DateTime? deadline);
        Task<bool> DeleteAsync(Guid userId, Guid taskId);

        Task<bool> MarkInProgressAsync(Guid userId, Guid taskId);
        Task<bool> CompleteAsync(Guid userId, Guid taskId);
    }
}
