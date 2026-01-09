using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId);
        Task<IReadOnlyList<TaskItem>> GetByUserAsync(
            Guid userId,
            TaskState? state,
            TaskPriority? priority);
        Task SaveChangesAsync();
    }
}
