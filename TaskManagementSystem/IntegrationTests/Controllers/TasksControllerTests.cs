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
            // Arrange
            var taskId = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync(
                $"/api/tasks/{taskId}/complete",
                content: null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

