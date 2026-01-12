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
                Title = "Integration task",
                Description = "Test description",
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
                Title = "My task",
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
    }
}
