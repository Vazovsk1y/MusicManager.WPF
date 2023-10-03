using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services.Storage;

public class MovieReleaseEntityJson : SerializableEntity<MovieRelease>
{
    public required string Identifier { get; set; }

    public string? ProductionCountry { get; set; }

    public int? ProductionYear { get; set; }

    public required string DiscType { get; set; }
}
