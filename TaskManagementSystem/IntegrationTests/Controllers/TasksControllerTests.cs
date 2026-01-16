using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagement.Application.DTOs;
using TaskManagement.IntegrationTests.Infrastructure;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.IntegrationTests.Controllers
{
    public class TasksControllerTests : IntegrationTestBase
    {
        public TasksControllerTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Create_ShouldReturn400_WhenTitleIsEmpty()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "",
                Priority = TaskPriority.Low
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ShouldReturn201_WhenDataIsValid()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Test Task Creation",
                Description = "Test description for successful task creation",
                Priority = TaskPriority.Medium,
                Deadline = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<TaskDto>();
            result.Should().NotBeNull();
            result!.Title.Should().Be(dto.Title);
            result.Priority.Should().Be(dto.Priority);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnOnlyCurrentUserTasks()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "User's Test Task",
                Description = "Test task for user retrieval method",
                Priority = TaskPriority.Medium
            };

            await _client.PostAsJsonAsync("/api/tasks", dto);

            // Act
            var response = await _client.GetAsync("/api/tasks");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            tasks.Should().HaveCount(1);
            tasks![0].Title.Should().Be(dto.Title);
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenTaskNotFound()
        {
            await ResetDatabaseAsync();

            // Arrange
            var taskId = Guid.NewGuid();
            var dto = new CreateTaskDto
            {
                Title = "Update non-existing task",
                Description = "Test update operation for non-existing task",
                Priority = TaskPriority.High
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/tasks/{taskId}",
                dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenTaskNotFound()
        {
            await ResetDatabaseAsync();

            // Arrange
            var taskId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync(
                $"/api/tasks/{taskId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task MarkInProgress_ShouldReturn404_WhenTaskNotFound()
        {
            await ResetDatabaseAsync();

            // Arrange
            var taskId = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync(
                $"/api/tasks/{taskId}/inprogress",
                content: null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Complete_ShouldReturn404_WhenTaskNotFound()
        {
            await ResetDatabaseAsync();

            // Arrange
            var taskId = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync(
                $"/api/tasks/{taskId}/complete",
                content: null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ShouldUpdateTask_WhenTaskExists()
        {
            await ResetDatabaseAsync();

            // Arrange
            var createDto = new CreateTaskDto
            {
                Title = "Test Task for Update",
                Description = "Initial task state before update operation",
                Priority = TaskPriority.Low
            };

            var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
            var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

            var updateDto = new CreateTaskDto
            {
                Title = "Updated Test Task",
                Description = "Task state after successful update operation",
                Priority = TaskPriority.High
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/tasks/{createdTask!.Id}",
                updateDto);

            var getResponse = await _client.GetAsync("/api/tasks");
            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            tasks.Should().HaveCount(1);
            tasks![0].Title.Should().Be(updateDto.Title);
            tasks[0].Priority.Should().Be(updateDto.Priority);
        }

        [Fact]
        public async Task Delete_ShouldRemoveTask_WhenTaskExists()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Test Task for Deletion",
                Description = "Task to test successful delete operation",
                Priority = TaskPriority.Medium
            };

            var createResponse = await _client.PostAsJsonAsync("/api/tasks", dto);
            var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

            // Act
            var deleteResponse = await _client.DeleteAsync(
                $"/api/tasks/{createdTask!.Id}");

            var getResponse = await _client.GetAsync("/api/tasks");
            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            tasks.Should().BeEmpty();
        }

        [Fact]
        public async Task MarkInProgress_ShouldChangeTaskState()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Test Task for InProgress",
                Description = "Task to test marking as in progress state",
                Priority = TaskPriority.Medium
            };

            var createResponse = await _client.PostAsJsonAsync("/api/tasks", dto);
            var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

            // Act
            var response = await _client.PostAsync(
                $"/api/tasks/{createdTask!.Id}/inprogress",
                null);

            var getResponse = await _client.GetAsync("/api/tasks");
            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            tasks![0].State.Should().Be(TaskState.InProgress);
        }

        [Fact]
        public async Task Complete_ShouldChangeTaskState()
        {
            await ResetDatabaseAsync();

            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Test Task for Completion",
                Description = "Task to test marking as completed state",
                Priority = TaskPriority.High
            };

            var createResponse = await _client.PostAsJsonAsync("/api/tasks", dto);
            var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

            // Act
            var response = await _client.PostAsync(
                $"/api/tasks/{createdTask!.Id}/complete",
                null);

            var getResponse = await _client.GetAsync("/api/tasks");
            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            tasks![0].State.Should().Be(TaskState.Completed);
        }
    }
}

