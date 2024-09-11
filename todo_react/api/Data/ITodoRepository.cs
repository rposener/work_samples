
namespace TodoApi.Data
{
    public interface ITodoRepository
    {
        Task<bool> AddTodoAsync(string todoList, TodoEntity entity);
        Task<bool> DeleteTodoAsync(string todoList, string rowKey);
        Task<IList<TodoEntity>> GetTodosAsync(string todoList);
        Task<bool> UpdateTodoAsync(string todoList, TodoEntity entity);
        Task<IList<string>> GetTodoListsAsync();
    }
}