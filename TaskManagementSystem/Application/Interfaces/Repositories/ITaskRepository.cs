using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(Guid taskId);
        Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId);
        IQueryable<TaskItem> QueryByUser(Guid userId);
        Task<IReadOnlyList<TaskItem>> ExecuteAsync(IQueryable<TaskItem> query);
        Task SaveChangesAsync();
    }
}
