using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services.Storage;

public class SongwriterEntityJson : SerializableEntity<Songwriter>
{
    public required string Name { get; set; }

    public required string LastName { get; set; }
}
