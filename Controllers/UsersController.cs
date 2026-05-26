using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            var createdUser = await _service.CreateUserAsync(userDto);
            return Ok(createdUser);
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