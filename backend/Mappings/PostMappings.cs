using instagram.DTOs;
using instagram.Models;

namespace instagram.Mappings;

public static class PostMappings
{
    public static PostReadDto ToDto(this Post post)
    {
        return new PostReadDto(
            post.Id,
            post.Descriere,
            post.ImageUrl,
            post.DataPublicarii,
            post.User?.UserName ?? "Utilizator Necunoscut" 
        );
    }
    public static List<PostReadDto> ToDtoList(this IEnumerable<Post> posts)
    {
        return posts.Select(p => p.ToDto()).ToList();
    }
    public static Post ToEntity(this CreatePostDto dto, string userId)
    {
        return new Post
        {
            Descriere = dto.Descriere,
            ImageUrl = dto.ImageUrl,
            DataPublicarii = DateTime.UtcNow, 
            UserId = userId 
        };
    }
}