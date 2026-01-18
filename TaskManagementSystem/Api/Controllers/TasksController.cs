using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IAuthorizationService _authorizationService;

        public TasksController(
            ITaskService taskService,
            IValidator<CreateTaskDto> createValidator,
            IAuthorizationService authorizationService)
        {
            _taskService = taskService;
            _createValidator = createValidator;
            _authorizationService = authorizationService;
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.Parse(userId!);
        }

        /// <summary>
        /// Create a new task for the current user
        /// </summary>
        /// <response code="201">Task successfully created</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
        {
            await ValidateAsync(dto);
            
            var userId = GetUserId();
            var task = await _taskService.CreateAsync(userId, dto);

            return CreatedAtAction(nameof(GetByUser), new { userId }, task);
        }

        /// <summary>
        /// Get tasks for the current authenticated user
        /// </summary>
        /// <param name="state">Filter tasks by state</param>
        /// <param name="priority">Filter tasks by priority</param>
        /// <param name="search">Search tasks by title</param>
        /// <param name="sort">Sorting option</param>
        /// <param name="skip">Number of items to skip</param>
        /// <param name="take">Number of items to take (max 100)</param>
        /// <returns>List of tasks</returns>
        /// <response code="200">Returns list of tasks for current user</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="400">Invalid request parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByUser(
            [FromQuery] TaskState? state = null,
            [FromQuery] TaskPriority? priority = null,
            [FromQuery] string? search = null,
            [FromQuery] TaskSortOption sort = TaskSortOption.CreatedAtDesc,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            if (take > 100)
                take = 100;

            var userId = GetUserId();
            var tasks = await _taskService.GetByUserAsync(
                userId, 
                state, 
                priority,
                search,
                sort,
                skip,
                take);

            return Ok(tasks);
        }

        /// <summary>
        /// Update task fields
        /// </summary>
        /// <response code="204">Task updated</response>
        /// <response code="404">Task not found</response>
        [HttpPut("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid taskId,
            [FromBody] CreateTaskDto dto)
        {
            await ValidateAsync(dto);

            var task = await _taskService.GetEntityByIdAsync(taskId);

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                task,
                "TaskAccessPolicy");

            if (!authResult.Succeeded)
                return Forbid();

            await _taskService.UpdateAsync(
                task.UserId,
                taskId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.Deadline);

            return NoContent();
        }

        /// <summary>
        /// Delete a task owned by the current user
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <response code="204">Task successfully deleted</response>
        /// <response code="404">Task not found</response>
        [HttpDelete("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            var task = await _taskService.GetEntityByIdAsync(taskId);

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                task,
                "TaskAccessPolicy");

            if (!authResult.Succeeded)
                return Forbid();

            await _taskService.DeleteAsync(task.UserId, taskId);

            return NoContent();
        }

        /// <summary>
        /// Mark task as InProgress
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <response code="204">Task state changed to InProgress</response>
        /// <response code="404">Task not found</response>
        /// <response code="400">Invalid task state transition</response>
        [HttpPost("{taskId}/inprogress")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkInProgress(Guid taskId)
        {
            var task = await _taskService.GetEntityByIdAsync(taskId);

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                task,
                "TaskAccessPolicy");

            if (!authResult.Succeeded)
                return Forbid();

            await _taskService.MarkInProgressAsync(task.UserId, taskId);
            
            return NoContent();
        }

        /// <summary>
        /// Mark task as Completed
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <response code="204">Task successfully completed</response>
        /// <response code="404">Task not found</response>
        /// <response code="400">Task is already completed</response>
        [HttpPost("{taskId}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Complete(Guid taskId)
        {
            var task = await _taskService.GetEntityByIdAsync(taskId);

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                task,
                "TaskAccessPolicy");

            if (!authResult.Succeeded)
                return Forbid();

            await _taskService.CompleteAsync(task.UserId, taskId);
            
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
