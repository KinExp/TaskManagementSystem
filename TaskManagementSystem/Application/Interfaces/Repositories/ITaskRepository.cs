using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
