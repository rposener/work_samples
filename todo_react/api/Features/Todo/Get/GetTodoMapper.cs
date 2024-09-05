using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Get
{
    public class GetTodoMapper : ResponseMapper<IEnumerable<TodoItem>, IEnumerable<TodoEntity>>
    {
        public override IEnumerable<TodoItem> FromEntity(IEnumerable<TodoEntity> e)
        {
            return e.Select(e => new TodoItem
            {
                Description = e.Description!,
                DueDate = e.DueDate.HasValue ? DateOnly.FromDateTime(e.DueDate.Value) : null,
                ReminderDays = e.ReminderDays,
                Id = e.RowKey,
                IsCompleted = e.IsCompleted
            });
        }
    }
}
