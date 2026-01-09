using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;

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
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByUser(
            [FromQuery] Guid userId,
            [FromQuery] TaskState? state = null,
            [FromQuery] TaskPriority? priority = null)
        {
            var tasks = await _taskService.GetByUserAsync(userId, state, priority);
            return Ok(tasks);
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        [HttpPut("{taskId}")]
        public async Task<IActionResult> Update(
            Guid taskId,
            [FromBody] CreateTaskDto dto)
        {
            var success = await _taskService.UpdateAsync(
                taskId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.Deadline);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            var success = await _taskService.DeleteAsync(taskId);
            if (!success)
                return NotFound();

            return NoContent();
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
