using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetTodos([FromQuery] string status)
        {
            int u_id = GetUserId();

            if (status == null)
            {
                var todos = _context.Todos.Where(todo =>
                todo.UserId == u_id);
                return Ok(todos);
            }
            bool check = false;
            if (status == "completed")
                check = true;
            else if (status != "pending")
                return BadRequest("Mention correct status");
            var filtered_todos = _context.Todos.Where(todos =>
                (todos.UserId == u_id) &&
              (todos.IsCompleted == check));
            return Ok(filtered_todos);

        }

       

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddToList(Todo todo)
        {
            int u_id = GetUserId();
            todo.UserId = u_id;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTodos",
            new { id = todo.Id },
            todo);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateTodo([FromRoute]int id,[FromBody] Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest("TodoItem Not Found");
            }
            int u_id = GetUserId();
            if (u_id != todo.UserId)
                return Unauthorized();
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
            var entity = await _context.Todos.FindAsync(id);
            if (entity == null)
                return NotFound();
            patchEntity.ApplyTo(entity, ModelState);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Todo>> DeleteTodo([FromRoute] int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();
            int u_id = GetUserId();
            if (u_id != todo.UserId)
                return Unauthorized();
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        private int GetUserId()
        {
            var currentUser = HttpContext.User;
            int u_id = 0;

            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                u_id = Int16.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            }

            return u_id;
        }
    }
}
