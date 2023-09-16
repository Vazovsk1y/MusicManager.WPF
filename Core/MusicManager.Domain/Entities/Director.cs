using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class Director
{
    private readonly List<Movie> _movies = new();

    public DirectorId Id { get; }

    public string FullName { get; private set; } = null!;

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

    private Director() 
    {
        Id = DirectorId.Create();
    }

    internal Result AddMovie(Movie movie)
    {
        if (_movies.SingleOrDefault(e => e.Id == movie.Id) is not null)
        {
            return Result.Failure(new("Passed movie is already added."));
        }

        _movies.Add(movie);
        return Result.Success();
    }

    public static Result<Director> Create(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure<Director>(DomainErrors.NullOrEmptyStringPassed("director fullName"));
        }

        return new Director() { FullName = fullName };
    }
}

public record DirectorId(Guid Value)
{
    public static DirectorId Create() => new DirectorId(Guid.NewGuid());
}