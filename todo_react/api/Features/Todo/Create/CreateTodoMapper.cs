using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Create
{
    public class CreateTodoMapper: Mapper<CreateTodoRequest, CreateTodoResponse, TodoEntity>
    {
        /// <summary>
        /// Returns an Entity from a CreateTodoRequest
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public override TodoEntity ToEntity(CreateTodoRequest r)
        {
            return new TodoEntity
            {
                Description = r.Description,
                DueDate = r.DueDate.HasValue ? new DateTime(r.DueDate.Value, TimeOnly.MinValue, DateTimeKind.Utc) : null,
                IsCompleted = r.IsCompleted,
                ReminderDays = r.ReminderDays
            };
        }

        /// <summary>
        /// Creates a Response from an Entity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public override CreateTodoResponse FromEntity(TodoEntity e)
        {
            return new CreateTodoResponse
            {
                Id = e.RowKey
            };
        }
    }
}
