using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.DTOs;
using instagram.Services;
namespace instagram.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LikesController:ControllerBase
{
    private readonly ILikeService _service;
    public LikesController(ILikeService service)
    {
        _service = service;
    }
    [HttpGet("post/{postId:int}")]
    [ProducesResponseType(typeof(List<LikeReadDto>),StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLikes(int postId)
    {
        var likes = await _service.GetLikesForPostAsync(postId);
        return Ok(likes);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LikeReadDto),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateLike([FromBody]CreateLikeDto createLikeDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try
        {
            var createdLike = await _service.CreateLikeAsync(createLikeDto, userId);
            if (createdLike == null) return NotFound();
            return CreatedAtAction(nameof(GetAllLikes), new { postId = createLikeDto.PostId }, createdLike);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLike(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var deleted = await _service.DeleteLikeAsync(id, currentUserId);
            if (!deleted) return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }
}