using FluentValidation;
using TaskManager.API.DTOs;

namespace TaskManager.API.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(100)
                .WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Priority)
                .NotEmpty()
                .WithMessage("Priority is required.")
                .Must(priority =>
                    priority == "Low" ||
                    priority == "Medium" ||
                    priority == "High")
                .WithMessage("Priority must be Low, Medium or High.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be in the future.");
        }
    }
}