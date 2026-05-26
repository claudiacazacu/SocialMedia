using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.Services;
namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _service;
    public FollowController(IFollowService service)
    {
        _service = service;
    }
    [HttpPost("{followingId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FollowUser(string followingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Unauthorized();

        try
        {
            var followed = await _service.FollowUserAsync(currentUserId, followingId);
            if (!followed) return NotFound("User not found.");
            return StatusCode(201, "Followed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("{followingId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnfollowUser(string followingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Unauthorized();

        var unfollowed = await _service.UnfollowUserAsync(currentUserId, followingId);
        if (!unfollowed) return NotFound("You are not following this user.");

        return NoContent();
    }
    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers(string userId)
    {
        var followers = await _service.GetFollowersAsync(userId);
        return Ok(followers);
    }
    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing(string userId)
    {
        var following = await _service.GetFollowingAsync(userId);
        return Ok(following);
    }
}