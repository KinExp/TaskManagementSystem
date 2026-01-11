using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId);
        Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId);
        Task<IReadOnlyList<TaskItem>> GetByUserAsync(
            Guid userId,
            TaskState? state,
            TaskPriority? priority);

        Task UpdateAsync(TaskItem task);
        Task RemoveAsync(TaskItem task);
        Task SaveChangesAsync();
    }
}
