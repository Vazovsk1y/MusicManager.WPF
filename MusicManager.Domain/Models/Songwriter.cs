using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Songwriter : Entity
{
    private readonly List<Movie> _movies = new();

    public Songwriter(Guid id, string name, string surname) : base(id) 
    {
        Name = name;
        Surname = surname;
    }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Name { get; private set; }

    public string Surname { get; private set; }

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();
}
