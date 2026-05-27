using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;
namespace instagram.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repository;
    private readonly ILogger<PostService> _logger;

    public PostService(IPostRepository repository, ILogger<PostService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync()
    {
        _logger.LogInformation("Fetching all posts");
        var posts = await _repository.GetAllAsync();
        return posts.ToDtoList();
    }

    public async Task<PostReadDto?> GetPostByIdAsync(int id)
    {
        _logger.LogInformation("Fetching post with id {PostId}", id);
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
            _logger.LogWarning("Post with id {PostId} not found", id);
        return post?.ToDto();
    }

    public async Task<PostReadDto?> CreatePostAsync(CreatePostDto dto, string userId)
    {
        _logger.LogInformation("Creating post for user {UserId}", userId);
        var post = dto.ToEntity(userId);
        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        var createdPost = await _repository.GetByIdAsync(post.Id);
        _logger.LogInformation("Post {PostId} created successfully for user {UserId}", post.Id, userId);
        return createdPost?.ToDto();
    }

    public async Task<PostReadDto?> UpdatePostAsync(int id, UpdatePostDto dto, string currentUserId, bool isAdmin)
    {
        _logger.LogInformation("Updating post {PostId} by user {UserId}", id, currentUserId);
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            _logger.LogWarning("Post {PostId} not found for update", id);
            return null;
        }
        if (!isAdmin && post.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to update post {PostId}", currentUserId, id);
            throw new UnauthorizedAccessException("n ai permisiune");
        }

        post.Descriere = dto.Descriere;
        post.ImageUrl = dto.ImageUrl;
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Post {PostId} updated successfully", id);
        return post.ToDto();
    }

    public async Task<bool> DeletePostAsync(int id, string currentUserId, bool isAdmin)
    {
        _logger.LogInformation("Deleting post {PostId} by user {UserId} (isAdmin={IsAdmin})", id, currentUserId, isAdmin);
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            _logger.LogWarning("Post {PostId} not found for deletion", id);
            return false;
        }
        if (!isAdmin && post.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to delete post {PostId}", currentUserId, id);
            throw new UnauthorizedAccessException("n ai permisiune");
        }
        await _repository.DeleteAsync(post);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Post {PostId} deleted successfully", id);
        return true;
    }
}
