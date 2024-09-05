using FastEndpoints;
using FluentValidation;
using System.Resources;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Create
{
    /// <summary>
    /// The Request Class for a new Todo Item
    /// </summary>
    public class CreateTodoRequest
    {
        public string? TodoList { get; set; }

        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }

        public int? ReminderDays { get; set; }

        public bool IsCompleted { get; set; }
        
    }


    /// <summary>
    /// The Validator for a Create Todo Request
    /// </summary>
    public class CreateTodoValidator : Validator<CreateTodoRequest>
    {
        public CreateTodoValidator()
        {
            RuleFor(r => r.TodoList)
                .NotEmpty()
                .WithMessage(Resources.Messages.Todo_List_Required);
            RuleFor(r => r.Description)
                .NotEmpty()
                .WithMessage(Resources.Messages.Todo_Description_Required);
            RuleFor(r => r.DueDate)
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(Resources.Messages.Todo_DueDate_InPast);
        }

    }

    /// <summary>
    /// The Response for a Create Todo Item
    /// </summary>
    public class CreateTodoResponse
    {
        public required string Id { get; set; }
    }
}
