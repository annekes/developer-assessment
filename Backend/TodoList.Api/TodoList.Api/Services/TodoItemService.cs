using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TodoList.Api.UnitTests")]
namespace TodoList.Api.Services
{
    /// <summary>
    /// Todo item service
    /// </summary>
    public class TodoItemService : ITodoItemService
    {
        private readonly TodoContext _context;

        public TodoItemService(TodoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a set of TodoItems considering whether items are complete
        /// </summary>
        /// <param name="getComplete">Whether to include complete TodoItems</param>
        /// <param name="getIncomplete">Whether to include incomplete TodoItems</param>
        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync(bool getComplete = true, bool getIncomplete = true)
        {
            // ASSUMPTION: Consumer may require a list of todo items that are complete and/or incomplete
            // Confirm with consumers/product managers if this is expected behaviour
            var todoItems = await _context.TodoItems.Where(todoItem => todoItem.IsCompleted!.Value ? getComplete : getIncomplete).ToListAsync();
            return todoItems;
        }

        /// <summary>
        /// Get a TodoItem from its id
        /// </summary>
        /// <param name="id">Guid attached to TodoItem</param>
        public async Task<TodoItem> GetTodoItemAsync(Guid id)
        {
            // Check whether id is valid/**/
            if (id == Guid.Empty)
            {
                return null;
            }
            return await _context.TodoItems.FindAsync(id);
        }

        /// <summary>
        /// Set new todo item
        /// </summary>
        /// <param name="todoItem">New todo item</param>
        public async Task<ValidTodoItem> SetTodoItemAsync(TodoItem todoItem)
        {
            // Check whether todo item exists
            if (todoItem == null)
            {
                return new ValidTodoItem(todoItem, false, "Todo item doesn't exist");
            }

            // Check whether id exists, create one if not
            // ASSUMPTION: Supply an id if sent without one
            // While not doing this for a pre-existing id for adding a new one, it is possible
            // Confirm with consumers/product managers if this is expected behaviour
            if (todoItem.Id == Guid.Empty)
            {
                todoItem.Id = Guid.NewGuid();
            }
            else if (TodoItemIdExists(todoItem.Id.Value))
            {
                return new ValidTodoItem(todoItem, false, "Id already exists");
            }

            // Check whether description exists on another todo item
            if (TodoItemDescriptionExists(todoItem.Id.Value, todoItem.Description))
            {
                return new ValidTodoItem(todoItem, false, "Description already exists");
            }

            // Check whether creating a pre-completed todo item
            // ASSUMPTION: Consumers should not be adding completed tasks to their list immediately
            // If there are database migrations, this protection should be considered as well
            if (todoItem.IsCompleted.Value)
            {
                return new ValidTodoItem(todoItem, false, "Todo item must be incomplete when created");
            }

            // Add new todo item
            _context.TodoItems.Add(todoItem);

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ValidTodoItem(todoItem, false, "Todo item couldn't be saved");
            }

            return new ValidTodoItem(todoItem, true);
        }

        /// <summary>
        /// Edit todo item
        /// </summary>
        /// <param name="id">Guid attached to TodoItem</param>
        /// <param name="todoItem">Updated TodoItem</param>
        public async Task<ValidTodoItem> EditTodoItem(Guid id, TodoItem todoItem)
        {
            // Check whether ids do not match
            if (id != todoItem.Id)
            {
                return new ValidTodoItem(todoItem, false, "Id does not match");
            }

            // Check whether new description already exists
            // ASSUMPTION: Because we care that a new todo item should not have a pre-existing description
            // We also care that we cannot edit a description to a pre-existing value
            if (TodoItemDescriptionExists(id, todoItem.Description))
            {
                return new ValidTodoItem(todoItem, false, "Description already exists");
            }

            // Check whether todo item already exists
            var originalTodoItem = await GetTodoItemAsync(id);
            if (originalTodoItem == null)
            {
                return new ValidTodoItem(todoItem, false, "Id does not exist");
            }

            // Check whether completed item is changing the description
            // ASSUMPTION: Should not be able to completely change the value of a todo item after it is completed
            // Clarification with consumers/product managers for specifics
            if (originalTodoItem.IsCompleted.Value && !string.Equals(originalTodoItem.Description, todoItem.Description))
            {
                return new ValidTodoItem(todoItem, false, "Cannot edit completed todo item");
            }

            // Check whether changing description on completion
            // ASSUMPTION: Should not be able to completely change the value of a todo item while completing it
            // Clarification with consumers/product managers for specifics
            if (todoItem.IsCompleted.Value && !originalTodoItem.IsCompleted.Value
                && !string.Equals(originalTodoItem.Description, todoItem.Description))
            {
                return new ValidTodoItem(todoItem, false, "Cannot edit description for a completed todo item");
            }

            // Update original todo item with new values
            originalTodoItem.Description = todoItem.Description;
            originalTodoItem.IsCompleted = todoItem.IsCompleted;

            _context.Entry(originalTodoItem).State = EntityState.Modified;

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ValidTodoItem(todoItem, false, "Todo item could not be saved");
            }

            return new ValidTodoItem(todoItem, true);
        }

        internal bool TodoItemIdExists(Guid id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        internal bool TodoItemDescriptionExists(Guid id, string description)
        {
            return _context.TodoItems
                    .Any(x =>
                        x.Description.ToLowerInvariant() == description.ToLowerInvariant()
                        && !x.IsCompleted!.Value
                        && x.Id.Value != id);
        }
    }
}
