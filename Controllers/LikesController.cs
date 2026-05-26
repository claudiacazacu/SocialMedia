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
public class LikesController:ControllerBase
{
    private readonly AppDbContext _context;
    public LikesController(AppDbContext context)
    {
        _context=context;
    }
    [HttpGet("post/{postId:int}")]
    [ProducesResponseType(typeof(List<LikeReadDto>),StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLikes(int postId)
    {
        var likes=await _context.Likes
            .Include(l=>l.User)
            .Where(l=>l.PostId==postId)
            .ToListAsync();
        return Ok(likes.ToDtoList());    
    }

[HttpPost]
[ProducesResponseType(typeof(LikeReadDto),StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> CreateLike([FromBody]CreateLikeDto createLikeDto)
{
    var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
    if(userId==null) return Unauthorized();
    var postExists=await _context.Posts.AnyAsync(p=>p.Id==createLikeDto.PostId);
    if(!postExists) return NotFound();
    var alreadyLiked=await _context.Likes.AnyAsync(l=>l.PostId==createLikeDto.PostId && l.UserId==userId);
    if(alreadyLiked) return BadRequest("You have already liked this post.");
    var like=createLikeDto.ToEntity(userId);
    _context.Likes.Add(like);
    await _context.SaveChangesAsync();
    var createdLike=await _context.Likes
        .Include(l=>l.User)
        .FirstOrDefaultAsync(l=>l.Id==like.Id);
    return CreatedAtAction(nameof(GetAllLikes),new {postId=like.PostId},createdLike!.ToDto());
}
[HttpDelete("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeleteLike(int id)
{
    var like=await _context.Likes.FindAsync(id);
    if(like==null) return NotFound();
    var currentUserId=User.FindFirstValue(ClaimTypes.NameIdentifier);
    if(like.UserId!=currentUserId)
    {
        return Forbid();
    }
    _context.Likes.Remove(like);
    await _context.SaveChangesAsync();
    return NoContent();
}
}