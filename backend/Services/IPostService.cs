using instagram.DTOs;
namespace instagram.Services;
public interface IPostService
{
    Task<IEnumerable<PostReadDto>> GetAllPostsAsync();
    Task<PostReadDto?> GetPostByIdAsync(int id);
    Task<PostReadDto?> CreatePostAsync(CreatePostDto dto, string userId);
    Task<PostReadDto?> UpdatePostAsync(int id, UpdatePostDto dto, string currentUserId, bool isAdmin);
    Task<bool> DeletePostAsync(int id, string currentUserId, bool isAdmin);
}