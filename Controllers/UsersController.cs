using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.DTOs;
using instagram.Services;
namespace instagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        
        public UsersController(IUserService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _service.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            var createdUser = await _service.CreateUserAsync(userDto);
            return Ok(createdUser);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Unauthorized();
            if (currentUserId != id && !User.IsInRole("Admin")) return Forbid();

            var updatedUser = await _service.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deleted = await _service.DeleteUserAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Userul nu a fost găsit." });
            }

            return Ok(new { message = "User șters cu succes de către administrator." });
        }
    }
}