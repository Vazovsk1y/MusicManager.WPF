using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services;

public class SongwriterEntityJson : SerializableEntity<Songwriter>
{
    public string Name { get; set; }

    public string LastName { get; set; } 
}
