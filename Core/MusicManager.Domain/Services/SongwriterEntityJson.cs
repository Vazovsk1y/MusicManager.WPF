using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services;

public class SongwriterEntityJson : SerializableEntityInfo<Songwriter>
{
    public string Name { get; set; }

    public string LastName { get; set; } 
}
