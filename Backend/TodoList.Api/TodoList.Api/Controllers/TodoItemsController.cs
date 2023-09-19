using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers
{
    //TODO: add inner methods that are testable
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _service;
        private readonly ILogger<TodoItemsController> _logger;

        /// <summary>
        /// Controller for todo items
        /// </summary>
        /// <param name="service">ITodoItemService</param>
        /// <param name="logger">ILogger<TodoItemsController></param>
        public TodoItemsController(ITodoItemService service, ILogger<TodoItemsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get list of todo items that are incomplete
        /// </summary>
        /// <example>
        /// GET: api/TodoItems
        /// </example>
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            _logger.Log(LogLevel.Information, "Get todo items.");
            var results = await _service.GetTodoItemsAsync(getComplete: false);
            return Ok(results);
        }

        /// <summary>
        /// Get todo item with id
        /// </summary>
        /// <param name="id">Guid for the todo item</param>
        /// <example>
        /// GET: api/TodoItems/...
        /// </example>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            _logger.Log(LogLevel.Information, "Get todo item. Id: {0}", id.ToString());
            var result = await _service.GetTodoItemAsync(id);

            if (result == null)
            {
                return NotFound("Todo item does not exist.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Edit todo item
        /// </summary>
        /// <param name="id">Guid of todo item to edit</param>
        /// <param name="todoItem">Modified todo item</param>
        /// <example>
        /// GET: api/TodoItems/...
        /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            _logger.Log(LogLevel.Information, "Put todo item. Id: {0}", id.ToString());
            var validTodoItem = await _service.EditTodoItem(id, todoItem);
            if (!validTodoItem.IsValid)
            {
                _logger.Log(LogLevel.Information, "Put todo item was invalid. Id: {0}", id.ToString());
                return BadRequest(validTodoItem.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Set todo item
        /// </summary>
        /// <param name="todoItem">Todo item to set</param>
        /// <example>
        /// POST: api/TodoItems
        /// </example>
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            _logger.Log(LogLevel.Information, "Post todo item.");
            var validTodoItem = await _service.SetTodoItemAsync(todoItem);
            if (!validTodoItem.IsValid)
            {
                _logger.Log(LogLevel.Information, "Post todo item was invalid.");
                return BadRequest(validTodoItem.Message);
            }
            return CreatedAtAction(nameof(GetTodoItem), new { id = validTodoItem.TodoItem.Id }, validTodoItem.TodoItem);
        }
    }
}
