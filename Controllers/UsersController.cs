using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using instagram.Data;
using instagram.Models;
using instagram.DTOs;

namespace instagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            var newUser = new ApplicationUser 
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                Nume = "NuAmNicioIdee",
                Prenume = "CredCaMergeOriceAici" 
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var returnDto = new UserReadDto
            {
                Id = newUser.Id,
                Username = newUser.UserName,
                Email = newUser.Email
            };
            return Ok(returnDto);
        }
    }
}