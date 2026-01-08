using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto);
        Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId);
        Task CompleteAsync(Guid taskId);
    }
}
