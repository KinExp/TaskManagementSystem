using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        }

        public async Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TaskItem>> GetByUserAsync(
            Guid userId,
            TaskState? state,
            TaskPriority? priority)
        {
            var query = _context.Tasks.Where(t => t.UserId == userId);

            if (state.HasValue)
                query = query.Where(t => t.State == state.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
