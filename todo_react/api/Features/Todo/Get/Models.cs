using FastEndpoints;

namespace TodoApi.Features.Todo.Get
{


    public class TodoItem
    {
        public required string Id { get; set; }

        public required string Description { get; set; }

        public DateOnly? DueDate { get; set; }

        public int? ReminderDays { get; set; }

        public bool IsCompleted { get; set; }
    }
}
