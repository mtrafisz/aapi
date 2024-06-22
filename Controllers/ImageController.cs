using Aapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    // update image by id
    // PUT: api/Image/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutImage(long id, Image image)
    {
        if (id != image.Id)
        {
            return BadRequest();
        }

        _context.Entry(image).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ImageExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
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

    // get image FILE by id
    // GET: api/Image/5/file
    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetImageFile(long id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), image.Url);
        var stream = System.IO.File.OpenRead(path);
        return File(stream, "image/png");
    }

    private bool ImageExists(long id)
    {
        return _context.Images.Any(e => e.Id == id);
    }
}
