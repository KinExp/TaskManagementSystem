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

        /// <summary>
        /// Создать новую задачу для пользователя
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(
            [FromQuery] Guid userId,
            [FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetByUser), new { userId }, task);
        }

        /// <summary>
        /// Получить все задачи пользователя
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByUser([FromQuery] Guid userId)
        {
            var tasks = await _taskService.GetByUserAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Отметить задачу как "InProgress"
        /// </summary>
        [HttpPost("{taskId}/inprogress")]
        public async Task<IActionResult> MarkInProgress(Guid taskId)
        {
            var success = await _taskService.MarkInProgressAsync(taskId);
            if (!success)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Завершить задачу
        /// </summary>
        [HttpPost("{taskId}/complete")]
        public async Task<IActionResult> Complete(Guid taskId)
        {
            var success = await _taskService.CompleteAsync(taskId);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
