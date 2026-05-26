using instagram.DTOs;
namespace instagram.Services;

public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
    Task<UserReadDto?> GetUserByIdAsync(string id);
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);
    Task<UserReadDto?> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(string id);
}
