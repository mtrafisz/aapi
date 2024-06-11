using Aapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly AapiDbContext _context;
    public ImageController(AapiDbContext context)
    {
        _context = context;
    }

    // create new image from file and anime id
    // POST: api/Image
    [HttpPost("{animeId}")]
    public async Task<ActionResult<Image>> PostImage(long animeId, Image.ImageType type, IFormFile file)
    {
        var anime = await _context.Animes.FindAsync(animeId);
        if (anime == null)
        {
            return NotFound();
        }

        var image = new Image
        {
            AnimeId = animeId,
            Type = type,
            Url = $"Data/Images/{Guid.NewGuid()}.png"
        };

        using var stream = new FileStream(image.Url, FileMode.Create);
        await file.CopyToAsync(stream);

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        anime.Images.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetImage", new { id = image.Id }, image);
    }

    // get image by id
    // GET: api/Image/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Image>> GetImage(long id)
    {
        var image = await _context.Images.FindAsync(id);

        if (image == null)
        {
            return NotFound();
        }

        return image;
    }

    // delete image by id
    // DELETE: api/Image/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImage(long id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
