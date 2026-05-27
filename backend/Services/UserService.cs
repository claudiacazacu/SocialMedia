using instagram.DTOs;
using instagram.Models;
using instagram.Repositories;

namespace instagram.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Fetching all users");
        var users = await _repository.GetAllAsync();
        return users.Select(u => new UserReadDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty));
    }

    public async Task<UserReadDto?> GetUserByIdAsync(string id)
    {
        _logger.LogInformation("Fetching user {UserId}", id);
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            _logger.LogWarning("User {UserId} not found", id);
        return user == null ? null : new UserReadDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
    {
        _logger.LogInformation("Creating user with username {Username}", dto.Username);
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Nume = dto.Nume,
            Prenume = dto.Prenume
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("User {UserId} created successfully", user.Id);
        return new UserReadDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
    }

    public async Task<UserReadDto?> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        _logger.LogInformation("Updating user {UserId}", id);
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for update", id);
            return null;
        }

        user.UserName = dto.Username;
        user.Email = dto.Email;
        user.Nume = dto.Nume;
        user.Prenume = dto.Prenume;

        await _repository.SaveChangesAsync();
        _logger.LogInformation("User {UserId} updated successfully", id);
        return new UserReadDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        _logger.LogInformation("Deleting user {UserId}", id);
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for deletion", id);
            return false;
        }

        await _repository.DeleteAsync(user);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("User {UserId} deleted successfully", id);
        return true;
    }
}
