namespace Aapi.Models;

using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

public class User : IdentityUser
{
    public string? Nickname { get; set; }
    public long? AvatarId { get; set; }

    [JsonIgnore]
    public Image? Avatar { get; set; }
    [JsonIgnore]
    public ICollection<Entry> Entries { get; set; } = new List<Entry>();
}

public class UserPublic
{
    public string? Nickname { get; set; }
    public long? AvatarId { get; set; }
    [JsonIgnore]
    public Image? Avatar { get; set; }
    public ICollection<Entry> Entries { get; set; } = new List<Entry>();
}

public class UserPatch
{
    public string? Nickname { get; set; }
}
