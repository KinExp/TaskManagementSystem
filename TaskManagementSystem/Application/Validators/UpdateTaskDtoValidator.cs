using FluentValidation;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200)
                .WithMessage("Title must be less than 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must be less than 1000 characters");

            RuleFor(x => x.Deadline)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage("Deadline must be in the future");
        }
    }
}
