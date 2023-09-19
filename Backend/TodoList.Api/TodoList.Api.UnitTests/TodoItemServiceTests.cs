using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Services;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemServiceTests
    {
        private readonly TodoContext _context;
        private TodoItem completeItem1 = new()
        {
            Id = Guid.Parse("08099398-0c61-4f1d-a170-9380328d510f"),
            Description = "Complete Item 1",
            IsCompleted = true
        };
        private TodoItem completeItem2 = new()
        {
            Id = Guid.Parse("d3de35a9-bb1b-4fc4-9ed3-740f556de170"),
            Description = "Complete Item 2",
            IsCompleted = true
        };
        private TodoItem incompleteItem1 = new()
        {
            Id = Guid.Parse("a362fcf2-3c83-4be3-9778-d71b1507797c"),
            Description = "Incomplete Item 1",
            IsCompleted = false
        };
        private TodoItem incompleteItem2 = new()
        {
            Id = Guid.Parse("6c5d118c-11cd-4344-bbdb-4f1e0a4d745a"),
            Description = "Incomplete Item 2",
            IsCompleted = false
        };
        private Guid newGuid = Guid.Parse("93eff679-3f76-421b-99bb-39ffe0ba2262");

        public TodoItemServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
            optionsBuilder.UseInMemoryDatabase("TodoItemsDB");
            _context = new TodoContext(optionsBuilder.Options);
            _context.Database.EnsureDeleted();
        }

        #region GetTodoItems

        [Fact]
        public async Task GetTodoItemsAsyncAllItems()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItems = await service.GetTodoItemsAsync();

            // assert
            Assert.Equal(4, todoItems.Count());
        }

        [Fact]
        public async Task GetTodoItemsAsyncCompleteItems()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItems = await service.GetTodoItemsAsync(getIncomplete: false);

            // assert
            Assert.Equal(2, todoItems.Count());
            Assert.Contains(todoItems, todoItem => todoItem.Id == completeItem1.Id);
            Assert.Contains(todoItems, todoItem => todoItem.Id == completeItem2.Id);
        }

        [Fact]
        public async Task GetTodoItemsAsyncIncompleteItems()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItems = await service.GetTodoItemsAsync(getComplete: false);

            // assert
            Assert.Equal(2, todoItems.Count());
            Assert.Contains(todoItems, todoItem => todoItem.Id == incompleteItem1.Id);
            Assert.Contains(todoItems, todoItem => todoItem.Id == incompleteItem2.Id);
        }

        [Fact]
        public async Task GetTodoItemsAsyncNone()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItems = await service.GetTodoItemsAsync(getIncomplete: false, getComplete: false);

            // assert
            Assert.Empty(todoItems);
        }

        #endregion

        #region GetTodoItem

        [Fact]
        public async Task GetTodoItemAsyncIncomplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItem = await service.GetTodoItemAsync(incompleteItem1.Id.Value);

            // assert
            Assert.Equal(incompleteItem1.Id.Value, todoItem.Id.Value);
        }

        [Fact]
        public async Task GetTodoItemAsyncComplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItem = await service.GetTodoItemAsync(completeItem1.Id.Value);

            // assert
            Assert.Equal(completeItem1.Id.Value, todoItem.Id.Value);
        }

        [Fact]
        public async Task GetTodoItemAsyncEmpty()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItem = await service.GetTodoItemAsync(Guid.Empty);

            // assert
            Assert.Null(todoItem);
        }

        [Fact]
        public async Task GetTodoItemAsyncDoesNotExist()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var todoItem = await service.GetTodoItemAsync(Guid.Parse("93eff679-3f76-421b-99bb-39ffe0ba2262"));

            // assert
            Assert.Null(todoItem);
        }

        #endregion

        #region SetTodoItem

        [Fact]
        public async Task SetTodoItemAsync()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = newGuid,
                Description = "New Todo Item",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.SetTodoItemAsync(newTodoItem);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.Equal(newTodoItem.Id.Value, validTodoItem.TodoItem.Id.Value);
        }

        [Fact]
        public async Task SetTodoItemAsyncWithNewId()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = Guid.Empty,
                Description = "New Todo Item",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.SetTodoItemAsync(newTodoItem);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.NotEqual(Guid.Empty, validTodoItem.TodoItem.Id.Value);
        }

        [Fact]
        public async Task SetTodoItemAsyncFailsBecauseComplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = newGuid,
                Description = "New Todo Item",
                IsCompleted = true
            };

            // act
            var validTodoItem = await service.SetTodoItemAsync(newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Todo item must be incomplete when created", validTodoItem.Message);
        }

        [Fact]
        public async Task SetTodoItemAsyncFailsBecauseSameId()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = completeItem1.Id.Value,
                Description = "New Todo Item",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.SetTodoItemAsync(newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Id already exists", validTodoItem.Message);
        }

        [Fact]
        public async Task SetTodoItemAsyncFailsBecauseSameDescription()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = newGuid,
                Description = incompleteItem1.Description,
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.SetTodoItemAsync(newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Description already exists", validTodoItem.Message);
        }

        #endregion

        #region EditTodoItem

        [Fact]
        public async Task EditTodoItemAsync()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = incompleteItem1.Id.Value,
                Description = "New Description",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.EditTodoItem(incompleteItem1.Id.Value, newTodoItem);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.Equal(newTodoItem.Id.Value, validTodoItem.TodoItem.Id.Value);
        }

        [Fact]
        public async Task EditTodoItemFailsIncorrectIdAsync()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = incompleteItem1.Id.Value,
                Description = "New Todo Item",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.EditTodoItem(newGuid, newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Id does not match", validTodoItem.Message);
        }

        [Fact]
        public async Task EditTodoItemFailsIdNotExistAsync()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = newGuid,
                Description = "New Todo Item",
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.EditTodoItem(newGuid, newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Id does not exist", validTodoItem.Message);
        }

        [Fact]
        public async Task EditTodoItemFailsExistingDescription()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = incompleteItem1.Id.Value,
                Description = incompleteItem2.Description,
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.EditTodoItem(incompleteItem1.Id.Value, newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Description already exists", validTodoItem.Message);
        }

        [Fact]
        public async Task EditTodoItemFailsChangeDescriptionAndComplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = incompleteItem1.Id.Value,
                Description = "New Description",
                IsCompleted = true
            };

            // act
            var validTodoItem = await service.EditTodoItem(incompleteItem1.Id.Value, newTodoItem);

            // assert
            Assert.False(validTodoItem.IsValid);
            Assert.Equal("Cannot edit description for a completed todo item", validTodoItem.Message);
        }

        [Fact]
        public async Task EditTodoItemPassChangingToComplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = incompleteItem1.Id.Value,
                Description = incompleteItem1.Description,
                IsCompleted = true
            };

            // act
            var validTodoItem = await service.EditTodoItem(incompleteItem1.Id.Value, newTodoItem);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.Equal(newTodoItem.Id.Value, validTodoItem.TodoItem.Id.Value);
        }

        [Fact]
        public async Task EditTodoItemPassChangingToIncomplete()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            var newTodoItem = new TodoItem()
            {
                Id = completeItem1.Id.Value,
                Description = completeItem1.Description,
                IsCompleted = false
            };

            // act
            var validTodoItem = await service.EditTodoItem(completeItem1.Id.Value, newTodoItem);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.Equal(newTodoItem.Id.Value, validTodoItem.TodoItem.Id.Value);
        }

        [Fact]
        public async Task EditTodoItemPassSameTodoItem()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(completeItem1);
            _context.TodoItems.Add(completeItem2);
            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var validTodoItem = await service.EditTodoItem(incompleteItem1.Id.Value, incompleteItem1);

            // assert
            Assert.True(validTodoItem.IsValid);
            Assert.Equal(incompleteItem1.Id.Value, validTodoItem.TodoItem.Id.Value);
        }

        #endregion

        #region TodoItemIdExists

        [Fact]
        public async Task TodoItemIdExists()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(completeItem1);
            await _context.SaveChangesAsync();

            // act
            var descriptionExists = service.TodoItemIdExists(completeItem1.Id.Value);

            // assert
            Assert.True(descriptionExists);
        }

        [Fact]
        public async Task TodoItemIdDoesNotExist()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(completeItem1);
            await _context.SaveChangesAsync();

            // act
            var descriptionExists = service.TodoItemIdExists(newGuid);

            // assert
            Assert.False(descriptionExists);
        }

        #endregion

        #region TodoItemDescriptionExists

        [Fact]
        public async Task TodoItemDescriptionDoesNotExistTest()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);
            await _context.SaveChangesAsync();

            var newDescription = "new description";

            // act
            var descriptionExists = service.TodoItemDescriptionExists(newGuid, newDescription);

            // assert
            Assert.False(descriptionExists);
        }

        [Fact]
        public async Task TodoItemDescriptionExistsCaseSensitiveTest()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);

            await _context.SaveChangesAsync();

            // act
            var descriptionExists = service.TodoItemDescriptionExists(newGuid, incompleteItem1.Description);

            // assert
            Assert.True(descriptionExists);
        }

        [Fact]
        public async Task TodoItemDescriptionExistsCaseInsensitiveTest()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);
            await _context.SaveChangesAsync();

            // act
            var descriptionExists = service.TodoItemDescriptionExists(newGuid, incompleteItem1.Description.ToLower());

            // assert
            Assert.True(descriptionExists);
        }

        [Fact]
        public async Task TodoItemDescriptionExistsCompleteTest()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(new TodoItem()
            {
                Id = Guid.Parse("48c9212f-2523-49f5-afdd-40d0b44a6d8c"),
                Description = "Same Description",
                IsCompleted = true
            });
            await _context.SaveChangesAsync();

            var newDescription = "Same Description";

            // act
            var descriptionExists = service.TodoItemDescriptionExists(newGuid, newDescription);

            // assert
            Assert.False(descriptionExists);
        }

        [Fact]
        public async Task TodoItemDescriptionExistsPassWithSameIdTest()
        {
            // arrange
            var service = new TodoItemService(_context);

            _context.TodoItems.Add(incompleteItem1);
            _context.TodoItems.Add(incompleteItem2);
            await _context.SaveChangesAsync();

            // act
            var descriptionExists = service.TodoItemDescriptionExists(incompleteItem1.Id.Value, incompleteItem1.Description);

            // assert
            Assert.False(descriptionExists);
        }

        #endregion
    }
}

