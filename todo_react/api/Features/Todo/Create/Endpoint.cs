using FastEndpoints;
using TodoApi.Data;

namespace TodoApi.Features.Todo.Create
{
    /// <summary>
    /// Create Todo Endpoint
    /// </summary>
    public class Endpoint : Endpoint<CreateTodoRequest, CreateTodoResponse, CreateTodoMapper>
    {
        private readonly ITodoRepository _todoRepository;

        public Endpoint(ITodoRepository todoRepository) 
        {
            _todoRepository = todoRepository;
        }

        public override void Configure()
        {
            Post("/todos");
            AllowAnonymous();
            Description(b => b.WithTags("Todos"));      
        }

        public override async Task HandleAsync(CreateTodoRequest req, CancellationToken ct)
        {
            // Map Request to Entity and Add to the Repository
            var entity = Map.ToEntity(req);
            var wasCreated = await _todoRepository.AddTodoAsync(req.TodoList!, entity);

            // Return error if not created
            if (!wasCreated)
            {
                await SendErrorsAsync(400, ct);
                return;
            }
            // return the response
            var response = Map.FromEntity(entity);
            await SendAsync(response, 200, ct);

        }
    }
}
