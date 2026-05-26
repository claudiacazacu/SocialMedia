using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using instagram.Data;
using instagram.Models;
namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly AppDbContext _context;
    public FollowController(AppDbContext context)
    {
        _context = context;
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
        if (currentUserId == followingId)
            return BadRequest("You cannot follow yourself.");
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == followingId);
        if (!userExists)
            return NotFound("User not found.");
        var alreadyFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == followingId);
        if (alreadyFollowing)
            return BadRequest("You already follow this user.");
        var follow = new Follow
        {
            FollowerId = currentUserId,
            FollowingId = followingId,
            Date = DateTime.UtcNow
        };
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        return StatusCode(201, "Followed successfully.");
    }
    [HttpDelete("{followingId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnfollowUser(string followingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Unauthorized();
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f =>
                f.FollowerId == currentUserId &&
                f.FollowingId == followingId);
        if (follow == null)
            return NotFound("You are not following this user.");

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers(string userId)
    {
        var followers = await _context.Follows
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .Select(f => f.Follower.UserName)
            .ToListAsync();

        return Ok(followers);
    }
    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing(string userId)
    {
        var following = await _context.Follows
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Following.UserName)
            .ToListAsync();

        return Ok(following);
    }
}