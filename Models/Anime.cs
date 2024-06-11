using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Aapi.Models;

public class Anime
{
    public long Id { get; set; }
    [Required]
    public string Titles { get; set; } = null!;
    [Required]
    public int Episodes { get; set; }
    public string? Genres { get; set; }
    public string? Studio { get; set; }

    [JsonIgnore]
    public ICollection<Image> Images { get; set; } = new List<Image>();
}

public class AnimePatch
{
    public string? Titles { get; set; }
    public int? Episodes { get; set; }
    public string? Genres { get; set; }
    public string? Studio { get; set; }
}

