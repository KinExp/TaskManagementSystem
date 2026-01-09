using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto);
        Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId);

        Task<bool> MarkInProgressAsync(Guid taskId);
        Task<bool> CompleteAsync(Guid taskId);
    }
}
