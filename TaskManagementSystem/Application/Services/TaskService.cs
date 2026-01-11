using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto)
        {
            if (dto.Deadline.HasValue && dto.Deadline.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Deadline cannot be in the past");
            }

            var task = new TaskItem(
                title: dto.Title,
                userId: userId,
                description: dto.Description,
                priority: dto.Priority,
                deadline: dto.Deadline);

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return MapToDto(task);
        }

        public async Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId)
        {
            var tasks = await _taskRepository.GetByUserAsync(userId);

            return tasks
                .Select(MapToDto)
                .ToList();
        }

        public async Task<IReadOnlyList<TaskDto>> GetByUserAsync(
            Guid userId,
            TaskState? state = null,
            TaskPriority? priority = null)
        {
            var tasks = await _taskRepository.GetByUserAsync(userId, state, priority);

            return tasks
                .Select(MapToDto)
                .ToList();
        }

        public async Task<bool> UpdateAsync(
            Guid userId,
            Guid taskId,
            string? title,
            string? description,
            TaskPriority? priority,
            DateTime? deadline)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null) 
                return false;

            if (deadline.HasValue && deadline.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Deadline cannot be in the past");

            if (!string.IsNullOrWhiteSpace(title))
                task.UpdateTitle(title);

            if (description != null) 
                task.UpdateDescription(description);

            if (priority.HasValue)
                task.UpdatePriority(priority.Value);

            if (deadline.HasValue)
                task.UpdateDeadline(deadline.Value);

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null) 
                return false;

            await _taskRepository.RemoveAsync(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkInProgressAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                return false;

            task.MarkInProgress();
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                return false;

            task.MarkCompleted();
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                State = task.State,
                Priority = task.Priority,
                Deadline = task.Deadline
            };
        }
    }
}
