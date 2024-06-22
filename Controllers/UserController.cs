using Aapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Aapi.Utility;

namespace Aapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly AapiDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserController(AapiDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("{nickname}")]
    public async Task<ActionResult<UserPublic>> GetUser(string nickname)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return NotFound();
        }
        return new UserPublic
        {
            Nickname = user.Nickname,
            AvatarId = user.AvatarId,
            Entries = user.Entries
        };
    }

    // patch current user
    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> PatchUser(UserPatch userPatch)
    {
        // get current user id:
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Nickname = userPatch.Nickname ?? user.Nickname;
        // email and password is handled by the auth API
        // avatar is handled by dedicated endpoint

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    // update current user avatar
    [HttpPut("avatar")]
    [Authorize]
    public async Task<IActionResult> PutAvatar(IFormFile file)
    {
        // get current user id:
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // delete old avatar
        if (user.AvatarId != null)
        {
            var oldAvatar = await _context.Images.FindAsync(user.AvatarId);
            if (oldAvatar != null)
            {
                _context.Images.Remove(oldAvatar);
                await _context.SaveChangesAsync();

                var old_path = Path.Combine(Directory.GetCurrentDirectory(), oldAvatar.Url);
                if (System.IO.File.Exists(old_path))
                {
                    System.IO.File.Delete(old_path);
                }
            }
        }

        // create new avatar
        var image = new Image
        {
            UserId = id,
            Type = Image.ImageType.Avatar,
            Url = $"Data/Images/Users/{id}/{Guid.NewGuid()}.png"
        };
        var directory = Path.GetDirectoryName(image.Url);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // save image file to disk
        var path = Path.Combine(Directory.GetCurrentDirectory(), image.Url);
        using (var stream = System.IO.File.Create(path))
        {
            ImageUtils.ResizeImage(ImageUtils.ImageFromIFormFile(file), 300, 300).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        user.AvatarId = image.Id;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    // get user avatar by nickname (FILE, not Image object - it's only for internal use)
    [HttpGet("{nickname}/avatar")]
    public async Task<IActionResult> GetAvatar(string nickname)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return NotFound();
        }

        var image = await _context.Images.FindAsync(user.AvatarId);
        if (image == null)
        {
            return NotFound();
        }

        return RedirectToAction("GetImageFile", "Image", new { id = image.Id });
    }
}