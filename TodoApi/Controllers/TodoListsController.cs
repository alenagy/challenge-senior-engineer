using System.Runtime.CompilerServices;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;

namespace TodoApi.Controllers
{
    [Route("api/todolists")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todolists
        [HttpGet]
        public async Task<ActionResult<IList<TodoList>>> GetTodoLists()
        {
            return Ok(await _context.TodoList.ToListAsync());
        }

        // GET: api/todolists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoList>> GetTodoList(long id)
        {
            var todoList = await _context.TodoList.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            return Ok(todoList);
        }

        // PUT: api/todolists/5
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
        {
            var todoList = await _context.TodoList.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            todoList.Name = payload.Name;
            await _context.SaveChangesAsync();

            return Ok(todoList);
        }

        // POST: api/todolists
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoList payload)
        {
            var todoList = new TodoList { UID = Guid.NewGuid(), LastUpdated = DateTime.Now, Name = payload.Name };

            _context.TodoList.Add(todoList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoList", new { id = todoList.Id }, todoList);
        }

        // DELETE: api/todolists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoList(long id)
        {
            var todoList = await _context.TodoList.FindAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }

            _context.TodoList.Remove(todoList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST api/todolists/:id/items/
        [HttpPost("{id}/items")]
        public async Task<ActionResult<TodoItem>> AddItemToList(long id, CreateItemInList payload)
        {
            var todoList = await _context.TodoList.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            TodoItem item = new TodoItem() { UID = Guid.NewGuid(), LastUpdated = DateTime.Now, Description = payload.Description, IsComplete = false, List = todoList };
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddItemToList", new { id = item.Id }, item);
        }

        // GET api/todolists/5/items/1
        [HttpGet("{id}/items/{itemId}")]
        public async Task<ActionResult<TodoItem>> GetItemFromList(long id, long itemId)
        {
            var todoItem = await this.GetTodoItemFromList(id, itemId);
            if (todoItem == null) {
                return NotFound();
            }

            return Ok(todoItem);
        }

        // PUT api/todolists/5/items/1
        [HttpPut("{id}/items/{itemId}")]
        public async Task<ActionResult<TodoItem>> UpdateItemInList(long id, long itemId, UpdateItemInList updatedItem)
        {
            var todoList = await _context.TodoList.FindAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }

            var todoItem = await this.GetTodoItemFromList(id, itemId);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Description = updatedItem.Description;
            todoItem.IsComplete = updatedItem.IsComplete;

            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        // DELETE api/todolists/5/items/1
        [HttpDelete("{id}/items/{itemId}")]
        public async Task<ActionResult> DeleteTodoItemFromList(long id, long itemId)
        {
            var itemToDelete = await this.GetTodoItemFromList(id, itemId);
            if (itemToDelete == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(itemToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoListExists(long id)
        {
            return (_context.TodoList?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<TodoItem?> GetTodoItemFromList(long listId, long itemId)
        {
            var todoItem = await _context.TodoItems.Include(i => i.List).FirstOrDefaultAsync(i => i.Id == itemId);
            if (todoItem?.List?.Id != listId) {
                return null;
            }

            return todoItem;
        }
    }
}
