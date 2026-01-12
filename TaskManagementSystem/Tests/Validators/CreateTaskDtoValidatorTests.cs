using System;
using FluentValidation.TestHelper;
using Xunit;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;
using TaskManagement.Domain.Enums;

namespace Tests.Validators
{
    public class CreateTaskDtoValidatorTests
    {
        private readonly CreateTaskDtoValidator _validator;

        public CreateTaskDtoValidatorTests()
        {
            _validator = new CreateTaskDtoValidator();
        }

        [Fact]
        public void ShouldHaveError_WhenTitleIsEmpty()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "",
                Priority = TaskPriority.Medium
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title is required");
        }

        [Fact]
        public void ShouldHaveError_WhenTitleTooLong()
        {
            var dto = new CreateTaskDto
            {
                Title = new string('a', 201),
                Priority = TaskPriority.Medium
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void ShouldHaveError_WhenDeadlineInPast()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Task",
                Priority = TaskPriority.Medium,
                Deadline = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Deadline)
                .WithErrorMessage("Deadline must be in the future");
        }

        [Fact]
        public void ShouldNotHaveErrors_WhenDtoIsValid()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Valid task",
                Description = "Description",
                Priority = TaskPriority.Medium,
                Deadline = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
