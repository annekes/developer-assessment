using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Api.Services
{
    /// <summary>
    /// Todo item service interface
    /// </summary>
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItem>> GetTodoItemsAsync(bool getComplete = true, bool getIncomplete = true);

        Task<TodoItem> GetTodoItemAsync(Guid id);

        Task<ValidTodoItem> SetTodoItemAsync(TodoItem todoItem);

        Task<ValidTodoItem> EditTodoItem(Guid id, TodoItem todoItem);
    }
}
