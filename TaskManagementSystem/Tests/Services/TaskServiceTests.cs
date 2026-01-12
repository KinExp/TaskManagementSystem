using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using TaskManagement.Application.Services;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Application.Exceptions;

namespace Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _repositoryMock;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _repositoryMock = new Mock<ITaskRepository>();
            _service = new TaskService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTask_WhenDataIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Desciption",
                Priority = TaskPriority.High,
                Deadline = DateTime.UtcNow.AddDays(1)
            };

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(userId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Priority, result.Priority);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByUserAsync_ShouldReturnTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var tasks = new List<TaskItem>
            {
                new TaskItem("Task 1", userId),
                new TaskItem("Task 2", userId)
            };

            _repositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _service.GetByUserAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task MarkInProgressAsync_ShouldChangeState_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new TaskItem("Task", Guid.NewGuid());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(task.Id, userId))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkInProgressAsync(userId, task.Id);

            // Assert
            Assert.Equal(TaskState.InProgress, task.State);
        }

        [Fact]
        public async Task MarkInProgressAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync((TaskItem?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.MarkInProgressAsync(userId, Guid.NewGuid()));
        }

        [Fact]
        public async Task CompleteAsync_ShouldChangeState_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new TaskItem("Task", Guid.NewGuid());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(task.Id, userId))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.CompleteAsync(userId, task.Id);

            // Assert
            Assert.Equal(TaskState.Completed, task.State);
        }

        [Fact]
        public async Task CompleteAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync((TaskItem?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CompleteAsync(userId, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask_WhenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new TaskItem("Old title", Guid.NewGuid());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(task.Id, userId))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(
                userId,
                task.Id,
                "New title",
                "New desciption",
                TaskPriority.Low,
                DateTime.UtcNow.AddDays(2));

            // Assert
            Assert.Equal("New title", task.Title);
            Assert.Equal(TaskPriority.Low, task.Priority);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync((TaskItem?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateAsync(
                    userId,
                    Guid.NewGuid(),
                    "title",
                    "description",
                    TaskPriority.High,
                    DateTime.UtcNow.AddDays(1)));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTrue_WhenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new TaskItem("Task", Guid.NewGuid());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(task.Id, userId))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.RemoveAsync(task))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(userId, task.Id);

            // Assert
            _repositoryMock.Verify(r => r.RemoveAsync(task), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync((TaskItem?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteAsync(userId, Guid.NewGuid()));
        }
    }
}