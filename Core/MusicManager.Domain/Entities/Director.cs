using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class Director
{
	#region --Fields--

	private readonly List<Movie> _movies = new();

	#endregion

	#region --Properties--

	public DirectorId Id { get; }

	public string FullName { get; private set; }

	public IReadOnlyCollection<Movie> Movies => _movies.ToList();

	#endregion

	#region --Constructors--

#pragma warning disable CS8618 
	private Director()
	{
		Id = DirectorId.Create();
	}
#pragma warning restore CS8618 


	#endregion

	#region --Methods--

	internal void AddMovie(Movie movie)
	{
		_movies.Add(movie);
	}

	internal void RemoveMovie(MovieId movieId)
	{
		var movie = _movies.SingleOrDefault(e => e.Id == movieId);
		if (movie is not null)
		{
			_movies.Remove(movie);
		}
	}

	public static Result<Director> Create(string fullName)
	{
		if (string.IsNullOrWhiteSpace(fullName))
		{
			return Result.Failure<Director>(DomainErrors.NullOrEmptyStringPassed("director fullName"));
		}

		return new Director() 
		{ 
			FullName = fullName 
		};
	}

	#endregion
}

public record DirectorId(Guid Value)
{
    public static DirectorId Create() => new(Guid.NewGuid());
}