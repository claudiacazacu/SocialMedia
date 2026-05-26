using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using instagram.Data;
using instagram.DTOs;
using instagram.Mappings;
namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public PostsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PostReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .OrderByDescending(p => p.DataPublicarii)
            .ToListAsync();
        return Ok(posts.ToDtoList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var post = createPostDto.ToEntity(userId);
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        
        var createdPost = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == post.Id);
            
        return CreatedAtAction(nameof(GetAllPosts), new { id = post.Id }, createdPost!.ToDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);   
        if (post == null) return NotFound();
        
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (post.UserId != currentUserId)
        {
            return Forbid();
        }
        
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}