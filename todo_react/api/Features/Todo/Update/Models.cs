using FastEndpoints;
using FluentValidation;

namespace TodoApi.Features.Todo.Update
{
    public class UpdateTodoRequest
    {
        public required string TodoList { get; set; }

        public required string Id { get; set; }

        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }

        public int? ReminderDays { get; set; }

        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// The Validator for a Create Todo Request
    /// </summary>
    public class CreateTodoValidator : Validator<UpdateTodoRequest>
    {
        public CreateTodoValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .WithMessage(Resources.Messages.Todo_Description_Required);
            RuleFor(r => r.DueDate)
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(Resources.Messages.Todo_DueDate_InPast);
        }
    }

    public class UpdateTodoResponse
    {
        public required string Id { get; set; }

        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }

        public int? ReminderDays { get; set; }

        public bool IsCompleted { get; set; }
    }
}
