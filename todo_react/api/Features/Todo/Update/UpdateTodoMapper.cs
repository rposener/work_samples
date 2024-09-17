using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Update
{
    public class UpdateTodoMapper : Mapper<UpdateTodoRequest, UpdateTodoResponse, TodoEntity>
    {
        public override TodoEntity ToEntity(UpdateTodoRequest r)
        {
            return new TodoEntity
            {
                RowKey = r.Id,
                Description = r.Description,
                DueDate = r.DueDate.HasValue ? new DateTime(r.DueDate.Value, TimeOnly.MinValue, DateTimeKind.Utc) : null,
                IsCompleted = r.IsCompleted,
                ReminderDays = r.ReminderDays
            };
        }

        public override UpdateTodoResponse FromEntity(TodoEntity e)
        {
            return new UpdateTodoResponse
            {
                Id = e.RowKey,
                Description = e.Description,
                DueDate = e.DueDate.HasValue ? DateOnly.FromDateTime(e.DueDate.Value) : null,
                IsCompleted = e.IsCompleted,
                ReminderDays = e.ReminderDays
            };
        }
    }
}
