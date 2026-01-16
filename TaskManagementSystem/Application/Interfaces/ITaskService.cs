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
            TaskPriority? priority = null,
            string? search = null,
            TaskSortOption sort = TaskSortOption.CreatedAtDesc,
            int skip = 0,
            int take = 20);

        Task UpdateAsync(
            Guid userId,
            Guid taskId,
            string? title,
            string? description,
            TaskPriority? priority,
            DateTime? deadline);
        Task DeleteAsync(Guid userId, Guid taskId);

        Task MarkInProgressAsync(Guid userId, Guid taskId);
        Task CompleteAsync(Guid userId, Guid taskId);
    }
}
