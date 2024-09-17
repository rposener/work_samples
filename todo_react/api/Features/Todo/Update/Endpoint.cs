using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Update
{
    public class Endpoint : Endpoint<UpdateTodoRequest, UpdateTodoResponse, UpdateTodoMapper>
    {
        private readonly ITodoRepository _todoRepository;

        public override void Configure()
        {
            Put("/todos/{todoList}");
            Description(b => b.WithTags("Todos"));
            Summary(s =>
            {
                s.Params["Id"] = "The Id of the Todo to Update";
                s.Params["TodoList"] = "The Todo List to Update";
                s.RequestParam(p => p.Description, "The Description of the Todo");
                s.RequestParam(p => p.DueDate, "The Due Date of the Todo");
                s.RequestParam(p => p.ReminderDays, "The Reminder Days of the Todo");
                s.RequestParam(p => p.IsCompleted, "The Completion Status of the Todo");
            });
        }

        public Endpoint(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// Handles updating the entity in the Repository
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public override async Task HandleAsync(UpdateTodoRequest req, CancellationToken ct)
        {
            var entity = Map.ToEntity(req);
            var updated = await _todoRepository.UpdateTodoAsync(req.TodoList!, entity);
            if (!updated)
            {
                await SendErrorsAsync(400, ct);
                return;
            }
            await SendAsync(Map.FromEntity(entity));

        }
    }
}
