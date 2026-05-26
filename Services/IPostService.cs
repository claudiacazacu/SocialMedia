using instagram.DTOs;
namespace instagram.Services;
public interface IPostService
{
    Task<IEnumerable<PostReadDto>> GetAllPostsAsync();
    Task<PostReadDto?> CreatePostAsync(CreatePostDto dto, string userId);
    Task<bool> DeletePostAsync(int id, string currentUserId, bool isAdmin);
}