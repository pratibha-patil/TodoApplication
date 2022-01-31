using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApplication.Models;

namespace TodoApplication.Controllers
{
    [Route("/signup")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodosContext _context;

        public UserController(TodosContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Registered User Successfully");
        }
    }
}
