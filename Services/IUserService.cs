using instagram.DTOs;
namespace instagram.Services;

public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);
    Task<bool> DeleteUserAsync(string id);
}
