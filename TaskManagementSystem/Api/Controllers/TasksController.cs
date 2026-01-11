using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;

namespace TaskManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IValidator<CreateTaskDto> _createValidator;

        public TasksController(
            ITaskService taskService,
            IValidator<CreateTaskDto> createValidator)
        {
            _taskService = taskService;
            _createValidator = createValidator;
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.Parse(userId!);
        }

        /// <summary>
        /// Создать новую задачу для пользователя
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
        {
            await ValidateAsync(dto);
            
            var userId = GetUserId();
            var task = await _taskService.CreateAsync(userId, dto);

            return CreatedAtAction(nameof(GetByUser), new { userId }, task);
        }

        /// <summary>
        /// Получить все задачи пользователя
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByUser(
            [FromQuery] TaskState? state = null,
            [FromQuery] TaskPriority? priority = null)
        {
            var userId = GetUserId();
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
            await ValidateAsync(dto);

            var userId = GetUserId();

            await _taskService.UpdateAsync(
                userId,
                taskId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.Deadline);

            return NoContent();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            var userId = GetUserId();

            await _taskService.DeleteAsync(userId, taskId);

            return NoContent();
        }

        /// <summary>
        /// Отметить задачу как "InProgress"
        /// </summary>
        [HttpPost("{taskId}/inprogress")]
        public async Task<IActionResult> MarkInProgress(Guid taskId)
        {
            var userId = GetUserId();

            await _taskService.MarkInProgressAsync(userId, taskId);
            
            return NoContent();
        }

        /// <summary>
        /// Завершить задачу
        /// </summary>
        [HttpPost("{taskId}/complete")]
        public async Task<IActionResult> Complete(Guid taskId)
        {
            var userId = GetUserId();

            await _taskService.CompleteAsync(userId, taskId);
            
            return NoContent();
        }

        private async Task ValidateAsync(CreateTaskDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                var message = string.Join("; ",
                    result.Errors.Select(e => e.ErrorMessage));

                throw new ValidationException(message);
            }
        }
    }
}
