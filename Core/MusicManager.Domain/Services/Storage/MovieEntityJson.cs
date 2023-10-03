using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services.Storage;

public class MovieEntityJson : SerializableEntity<Movie>
{
    public required string Title { get; set; }

    public string? ProductionCountry { get; set; }

    public required int ProductionYear { get; set; }
}
