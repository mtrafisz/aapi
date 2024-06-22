using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Aapi.Models;

public class Image
{
    public long Id { get; set; }
    public long? AnimeId { get; set; }
    public string? UserId { get; set; }     // string because IdentityUser.Id is a string for some reason
    [Required]
    public string Url { get; set; } = null!;
    
    public enum ImageType
    {
        Poster,
        Cover,
        Screenshot,
        Avatar
    }
    [Required]
    public ImageType Type { get; set; }
    
    [JsonIgnore]
    public Anime? Anime { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}
