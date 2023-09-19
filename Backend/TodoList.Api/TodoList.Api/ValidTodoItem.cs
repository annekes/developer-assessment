namespace TodoList.Api
{
    /// <summary>
    /// Validation of todo items
    /// </summary>
    public class ValidTodoItem
    {
        /// <summary>
        /// Constructor for a valid todo item
        /// </summary>
        /// <param name="todoItem">Todo item that was validated</param>
        /// <param name="isValid">Whether todo item is valid</param>
        /// <param name="message">Why todo item is invalid</param>
        public ValidTodoItem(TodoItem todoItem, bool isValid, string message = "")
        {
            TodoItem = todoItem;
            IsValid = isValid;
            Message = message;
        }

        public TodoItem TodoItem { get; set; }

        public bool IsValid { get; set; }

        public string Message { get; set; }


    }
}
