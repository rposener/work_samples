using FastEndpoints;
using TodoApi.Data;
using TodoApi.Features.Todo.Get;

namespace TodoApi.Features.Todo.List
{
    public class Endpoint : EndpointWithoutRequest<IEnumerable<string>>
    {
        private readonly ITodoRepository _todoRepository;

        public override void Configure()
        {
            Get("/todos");
            AllowAnonymous();
            Description(b => b.WithTags("Todos"));
            Summary(s =>
            {
                s.Params["TodoList"] = "List all Todo Lists";
                //s.RequestParam(p => p.TodoList, "Todo List to Retrieve");
            });
        }

        public Endpoint(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            // Load the Todo listss from the Respository
            var entities = await _todoRepository.GetTodoListsAsync();

            // Send the Response
            await SendAsync(entities, 200, ct);
        }
    }
}
