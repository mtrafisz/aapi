using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Aapi.Models;

public class Entry
{
    public long Id { get; set; }
    [Required]
    public long AnimeId { get; set; }
    [Required]
    public string UserId { get; set; } = null!;     // string because IdentityUser.Id is a string for some reason
    [Required]
    [Range(1, 10)]
    public int Rating { get; set; }

    [JsonIgnore]
    public Anime Anime { get; set; } = null!;
    [JsonIgnore]
    public User User { get; set; } = null!;
}

public class  EntryPatch
{
    [Range(1,10)]
    public int? Rating { get; set; }
}
