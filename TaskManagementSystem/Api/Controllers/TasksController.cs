using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(
            Guid userId,
            CreateTaskDto dto)
        {
            var task = await _taskService.CreateAsync(userId, dto);
            return Ok(task);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByUser(Guid userId)
        {
            var tasks = await _taskService.GetByUserAsync(userId);
            return Ok(tasks);
        }

        [HttpPost("{taskId}/complete")]
        public async Task<IActionResult> Complete(Guid taskId)
        {
            await _taskService.CompleteAsync(taskId);
            return NoContent();
        }
    }
}
