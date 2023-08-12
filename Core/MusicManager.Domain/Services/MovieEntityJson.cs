using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services;

public class MovieEntityJson : SerializableEntity<Movie>
{
    public string Title { get; set; }

    public string? ProductionCountry { get; set; }

    public int? ProductionYear { get; set; }

    public string? DirectorName { get; set; }

    public string? DirectorLastName { get; set; }
}
