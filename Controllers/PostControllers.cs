using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.DTOs;
using instagram.Services;

namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PostReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _service.GetAllPostsAsync();
        return Ok(posts);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var createdPost = await _service.CreatePostAsync(createPostDto, userId);
        if (createdPost == null) return BadRequest();

        return CreatedAtAction(nameof(GetAllPosts), new { id = createdPost.Id }, createdPost);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var deleted = await _service.DeletePostAsync(id, currentUserId, false);
            if (!deleted) return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeletePost(int id)
    {
        var deleted = await _service.DeletePostAsync(id, string.Empty, true);
        if (!deleted) return NotFound();

        return NoContent();
    }
}