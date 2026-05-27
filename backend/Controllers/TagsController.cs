using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using instagram.Data;
using instagram.Models;

namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<TagsController> _logger;

    public TagsController(AppDbContext context, ILogger<TagsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        _logger.LogInformation("Fetching all tags");
        var tags = await _context.Tags
            .Select(t => new { t.Id, t.Name })
            .ToListAsync();
        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTag(int id)
    {
        _logger.LogInformation("Fetching tag {TagId}", id);
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            _logger.LogWarning("Tag {TagId} not found", id);
            return NotFound();
        }
        return Ok(new { tag.Id, tag.Name });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
    {
        _logger.LogInformation("Admin creating tag '{TagName}'", dto.Name);
        var exists = await _context.Tags.AnyAsync(t => t.Name == dto.Name);
        if (exists)
            return BadRequest(new { Message = "Tag-ul există deja." });

        var tag = new Tag { Name = dto.Name };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Tag {TagId} '{TagName}' created", tag.Id, tag.Name);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new { tag.Id, tag.Name });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        _logger.LogInformation("Admin deleting tag {TagId}", id);
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            _logger.LogWarning("Tag {TagId} not found for deletion", id);
            return NotFound();
        }
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Tag {TagId} deleted", id);
        return NoContent();
    }
}

public record CreateTagDto(string Name);
