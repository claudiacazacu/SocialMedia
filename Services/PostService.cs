using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;
namespace instagram.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repository;

    public PostService(IPostRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync()
    {
        var posts = await _repository.GetAllAsync();
        return posts.ToDtoList();
    }

    public async Task<PostReadDto?> GetPostByIdAsync(int id)
    {
        var post = await _repository.GetByIdAsync(id);
        return post?.ToDto();
    }

    public async Task<PostReadDto?> CreatePostAsync(CreatePostDto dto, string userId)
    {
        var post = dto.ToEntity(userId);
        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        var createdPost = await _repository.GetByIdAsync(post.Id);
        return createdPost?.ToDto();
    }

    public async Task<PostReadDto?> UpdatePostAsync(int id, UpdatePostDto dto, string currentUserId, bool isAdmin)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null) return null;
        if (!isAdmin && post.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("n ai permisiune");
        }

        post.Descriere = dto.Descriere;
        post.ImageUrl = dto.ImageUrl;
        await _repository.SaveChangesAsync();

        return post.ToDto();
    }

    public async Task<bool> DeletePostAsync(int id, string currentUserId, bool isAdmin)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null) return false; 
        if (!isAdmin && post.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("n ai permisiune");
        }
        await _repository.DeleteAsync(post);
        await _repository.SaveChangesAsync();
        return true;
    }
}