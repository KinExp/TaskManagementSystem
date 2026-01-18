using TaskManagement.Application.DTOs;
using TaskManagement.Application.Exceptions;
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
            TaskPriority? priority = null,
            string? search = null,
            TaskSortOption sort = TaskSortOption.CreatedAtDesc,
            int skip = 0,
            int take = 20)
        {
            var tasks = await _taskRepository.GetByUserAsync(
                userId,
                state,
                priority,
                search,
                sort,
                skip,
                take);

            return tasks
                .Select(MapToDto)
                .ToList();
        }

        public async Task UpdateAsync(
            Guid userId,
            Guid taskId,
            string? title,
            string? description,
            TaskPriority? priority,
            DateTime? deadline)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                throw new NotFoundException("Task not found");

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
        }

        public async Task DeleteAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                throw new NotFoundException("Task not found");

            await _taskRepository.RemoveAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task MarkInProgressAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                throw new NotFoundException("Task not found");

            task.MarkInProgress();
            await _taskRepository.SaveChangesAsync();
        }

        public async Task CompleteAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);
            if (task == null)
                throw new NotFoundException("Task not found");

            task.MarkCompleted();
            await _taskRepository.SaveChangesAsync();
        }

        public async Task<TaskItem> GetEntityByIdAsync(Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
                throw new NotFoundException("Task not found");

            return task;
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
