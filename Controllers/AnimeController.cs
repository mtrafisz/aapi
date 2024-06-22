using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aapi.Models;
using Aapi.Utility;


namespace Aapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimeController : ControllerBase
{
    private readonly AapiDbContext _context;

    public AnimeController(AapiDbContext context)
    {
        _context = context;
    }

    // GET: api/Anime
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes()
    {
        return await _context.Animes.ToListAsync();
    }

    // GET: api/Anime/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Anime>> GetAnime(long id)
    {
        var anime = await _context.Animes.FindAsync(id);

        if (anime == null)
        {
            return NotFound();
        }

        return anime;
    }

    // PUT: api/Anime/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnime(long id, Anime anime)
    {
        if (id != anime.Id)
        {
            return BadRequest();
        }

        _context.Entry(anime).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimeExists(id))
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

    // POST: api/Anime
    [HttpPost]
    public async Task<ActionResult<Anime>> PostAnime(Anime anime)
    {
        _context.Animes.Add(anime);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAnime", new { id = anime.Id }, anime);
    }

    // DELETE: api/Anime/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnime(long id)
    {
        var anime = await _context.Animes.FindAsync(id);
        if (anime == null)
        {
            return NotFound();
        }

        _context.Animes.Remove(anime);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PATCH: api/Anime/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAnime(long id, [FromBody] AnimePatch animePatch)
    {
        if (animePatch == null)
        {
            return BadRequest(new { message = "Invalid JSON" });
        }

        var anime = await _context.Animes.FindAsync(id);
        if (anime == null)
        {
            return NotFound();
        }

        if (animePatch.Titles != null) anime.Titles = animePatch.Titles;
        if (animePatch.Episodes != null) anime.Episodes = animePatch.Episodes.Value;
        if (animePatch.Genres != null) anime.Genres = animePatch.Genres;
        if (animePatch.Studio != null) anime.Studio = animePatch.Studio;

        await _context.SaveChangesAsync();
        return Ok(anime);
    }

    // GET: api/Anime/5/Images
    [HttpGet("{id}/Images")]
    public async Task<ActionResult<IEnumerable<Models.Image>>> GetImages(long id)
    {
        var anime = await _context.Animes.FindAsync(id);
        if (anime == null)
        {
            return NotFound();
        }

        return await _context.Images.Where(i => i.AnimeId == id).ToListAsync();
    }

    // POST: api/Anime/5/Images
    [HttpPost("{id}/Images")]
    public async Task<ActionResult<Models.Image>> PostImage(long id, Models.Image.ImageType type, IFormFile file)
    {
        var anime = await _context.Animes.FindAsync(id);
        if (anime == null)
        {
            return NotFound();
        }

        var image = new Models.Image
        {
            AnimeId = id,
            Type = type,
            Url = $"Data/Images/Anime/{anime.Id}/{Guid.NewGuid()}.png"
        };

        var directory = Path.GetDirectoryName(image.Url);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        ImageUtils.ResizeImage(ImageUtils.ImageFromIFormFile(file), 300, 500).Save(image.Url, System.Drawing.Imaging.ImageFormat.Png);

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetImage", new { id = image.Id }, image);
    }

    private bool AnimeExists(long id)
    {
        return _context.Animes.Any(e => e.Id == id);
    }
}
