using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using instagram.DTOs;
using instagram.Services;

namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentsController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet("post/{postId:int}")]
    [ProducesResponseType(typeof(List<CommentReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommentsForPost(int postId)
    {
        var comments = await _service.GetCommentsForPostAsync(postId);
        return Ok(comments);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CommentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComment(int id)
    {
        var comment = await _service.GetCommentByIdAsync(id);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CommentReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var createdComment = await _service.CreateCommentAsync(createCommentDto, userId);
        if (createdComment == null) return NotFound();

        return CreatedAtAction(nameof(GetCommentsForPost), new { postId = createCommentDto.PostId }, createdComment);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CommentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var updatedComment = await _service.UpdateCommentAsync(id, updateCommentDto, currentUserId);
            if (updatedComment == null) return NotFound();
            return Ok(updatedComment);
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
    public async Task<IActionResult> DeleteComment(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null) return Unauthorized();

        try
        {
            var deleted = await _service.DeleteCommentAsync(id, currentUserId);
            if (!deleted) return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }
}