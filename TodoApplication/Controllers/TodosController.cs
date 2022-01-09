using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApplication.Models;

namespace TodoApplication.Controllers
{
    [Route("/todos")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodosContext _context;

        public TodosController(TodosContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetTodos([FromQuery] string status)
        {
            if (status == null)
            {
                var todos = await _context.Todos.ToListAsync();
                return Ok(todos);
            }
            bool check=false;
            if (status == "completed")
                check = true;
            else if (status != "pending")
                return BadRequest("Mention correct status");
            var filtered_todos = _context.Todos.Where(todos =>
              todos.IsCompleted == check);
            return Ok(filtered_todos);
            
        }


        [HttpPost]
        public async Task<IActionResult> AddToList(Todo todo)
        {
            _context.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTodos",
            new { id = todo.Id },
            todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo([FromRoute]int id,[FromBody] Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }
            _context.Entry(todo).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Todos.Find(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id,[FromBody] JsonPatchDocument<Todo> patchEntity)
        {
            var entity = await _context.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
            if (entity == null)
                return NotFound();
            patchEntity.ApplyTo(entity, ModelState);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Todo>> DeleteTodo([FromRoute] int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return todo;
        }
    }
}
