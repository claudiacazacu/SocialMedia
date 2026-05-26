using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.DTOs;
using instagram.Services;

namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RepostsController : ControllerBase
{
    private readonly IRepostService _service;

    public RepostsController(IRepostService service)
    {
        _service = service;
    }

    [HttpGet("post/{postId:int}")]
    [ProducesResponseType(typeof(List<RepostReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRepostsForPost(int postId)
    {
        var reposts = await _service.GetRepostsForPostAsync(postId);
        return Ok(reposts);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepostReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRepost([FromBody] CreateRepostDto createRepostDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var createdRepost = await _service.CreateRepostAsync(createRepostDto, userId);
        if (createdRepost == null) return NotFound();

        return CreatedAtAction(nameof(GetRepostsForPost), new { postId = createdRepost.PostId }, createdRepost);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRepost(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var deleted = await _service.DeleteRepostAsync(id, currentUserId);
            if (!deleted) return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }
}
