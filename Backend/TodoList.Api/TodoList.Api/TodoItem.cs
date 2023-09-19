using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Api
{
    /// <summary>
    /// Todo item
    /// </summary>
    public class TodoItem
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool? IsCompleted { get; set; }
    }
}
