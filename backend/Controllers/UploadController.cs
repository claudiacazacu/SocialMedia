using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace instagram.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "Niciun fișier selectat." });

        if (file.Length > MaxFileSize)
            return BadRequest(new { Message = "Fișierul depășește limita de 5 MB." });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { Message = "Tip de fișier neacceptat. Folosește JPG, PNG, GIF sau WEBP." });

        var imagesFolder = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(imagesFolder);

        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(imagesFolder, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation("Image uploaded: {FileName}", uniqueName);

        var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{uniqueName}";
        return Ok(new { imageUrl });
    }
}
