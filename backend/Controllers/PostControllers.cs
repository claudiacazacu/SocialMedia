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

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<PostReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsByUser(string userId)
    {
        var posts = await _service.GetPostsByUserIdAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(int id)
    {
        var post = await _service.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
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

        return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var updatedPost = await _service.UpdatePostAsync(id, updatePostDto, currentUserId, User.IsInRole("Admin"));
            if (updatedPost == null) return NotFound();
            return Ok(updatedPost);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
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