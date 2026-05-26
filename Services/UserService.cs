using instagram.DTOs;
using instagram.Models;
using instagram.Repositories;

namespace instagram.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(u => new UserReadDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty));
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Nume = dto.Nume,
            Prenume = dto.Prenume
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return new UserReadDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        await _repository.DeleteAsync(user);
        await _repository.SaveChangesAsync();
        return true;
    }
}
