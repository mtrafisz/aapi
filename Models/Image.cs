using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Aapi.Models;

public class Image
{
    public long Id { get; set; }
    [Required]
    public long AnimeId { get; set; }
    [Required]
    public string Url { get; set; } = null!;
    
    public enum ImageType
    {
        Poster,
        Cover,
        Screenshot
    }
    [Required]
    public ImageType Type { get; set; }

    [JsonIgnore]
    public Anime Anime { get; set; } = null!;
}
