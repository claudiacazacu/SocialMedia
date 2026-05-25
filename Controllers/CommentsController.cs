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
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("post/{postId:int}")]
    [ProducesResponseType(typeof(List<CommentReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommentsForPost(int postId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.Date)
            .ToListAsync();

        return Ok(comments.ToDtoList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(CommentReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var postExists = await _context.Posts.AnyAsync(p => p.Id == createCommentDto.PostId);
        if (!postExists) return NotFound();

        var comment = createCommentDto.ToEntity(userId);
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var createdComment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        return CreatedAtAction(nameof(GetCommentsForPost), new { postId = comment.PostId }, createdComment!.ToDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (comment.UserId != currentUserId)
        {
            return Forbid();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}