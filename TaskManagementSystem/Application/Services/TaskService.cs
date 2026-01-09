using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;

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

        public async Task<bool> MarkInProgressAsync(Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return false;

            task.MarkInProgress();
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteAsync(Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
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
