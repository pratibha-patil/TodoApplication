using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Models;

namespace TodoApplication.Controllers
{
    
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodosContext _context;
        private IConfiguration _config;

        public UserController(TodosContext context ,IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Route("/signup")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Registered User Successfully");
        }

        [Route("/login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            IActionResult response = Unauthorized();
            var logged_in_user = AuthenticateUser(user);

            if (logged_in_user != null)
            {
                var tokenString = GenerateJSONWebToken(logged_in_user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private object GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                new[] {
                new Claim("UserName", user.Name),
                new Claim("UserId",user.Id.ToString())
                 }
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token =(JwtSecurityToken)tokenHandler.CreateJwtSecurityToken(issuer: _config["Jwt:Issuer"], audience: _config["Jwt:Issuer"],
                subject: claimsIdentity, notBefore: DateTime.Now, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        private User AuthenticateUser(User user)
        {

            User u = _context.Users.Where(item => item.Name == user.Name).FirstOrDefault();
            User new_user = null;
            if (u == null)
                return u;
            if (u.Name == user.Name && u.Password == user.Password)
            {
                new_user= new User {Id=u.Id, Name = u.Name };
            }
            return new_user;
        }
    }
}
