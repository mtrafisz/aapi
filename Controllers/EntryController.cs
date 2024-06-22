using Aapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntryController : Controller
{
    private readonly AapiDbContext _context;
    public EntryController(AapiDbContext context)
    {
        _context = context;
    }

    // create new entry for user and anime and rating
    // POST: api/Entry
    [HttpPost]
    public async Task<ActionResult<Entry>> PostEntry(Entry entry)
    {
        _context.Entries.Add(entry);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEntry", new { id = entry.Id }, entry);
    }

    // get entry by entry id
    // GET: api/Entry/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Entry>> GetEntry(long id)
    {
        var entry = await _context.Entries.FindAsync(id);

        if (entry == null)
        {
            return NotFound();
        }

        return entry;
    }

    // delete entry by entry id
    // DELETE: api/Entry/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntry(long id)
    {
        var entry = await _context.Entries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        _context.Entries.Remove(entry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // update entry by id
    // PATCH: api/Entry/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchEntry(long id, EntryPatch entryPatch)
    {
        var entry = await _context.Entries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        if (entryPatch.Rating != null)
        {
            entry.Rating = entryPatch.Rating.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EntryExists(long id)
    {
        return _context.Entries.Any(e => e.Id == id);
    }
}
