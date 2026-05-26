using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
                .Select(u => new UserReadDto(
                    u.Id, 
                    u.UserName!, 
                    u.Email!
                )).ToListAsync();
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
            (
                newUser.Id,
                newUser.UserName!, 
                newUser.Email!
            );
            return Ok(returnDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Userul nu a fost găsit." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User șters cu succes de către administrator." });
        }
    }
}