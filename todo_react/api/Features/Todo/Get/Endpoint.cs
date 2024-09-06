using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Get
{
    public class Endpoint : EndpointWithoutRequest<IEnumerable<TodoItem>, GetTodoMapper>
    {
        private readonly ITodoRepository _todoRepository;

        public override void Configure()
        {
            Get("/todos/{todoList}");
            AllowAnonymous();
            Description(b => b.WithTags("Todos"));
            Summary(s =>
            {
                s.Params["TodoList"] = "Todo List to retrieve";
                //s.RequestParam(p => p.TodoList, "Todo List to Retrieve");
            });
        }

        public Endpoint(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            // Get the Todo List from the route
            var todoList = Route<string>("TodoList", true);
            if (todoList == null)
            {
                await SendAsync(Enumerable.Empty<TodoItem>(), 200, ct);
                return;
            }

            // Load the Todos from the Respository
            var entities = await _todoRepository.GetTodosAsync(todoList);
            var response = Map.FromEntity(entities);

            // Send the Response
            await SendAsync(response, 200, ct);
        }
    }
}
