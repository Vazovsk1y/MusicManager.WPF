using MusicManager.Domain.Enums;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.WPF.ViewModels.Entities;
using System.Linq;

namespace MusicManager.WPF.Infrastructure;

internal static class Mapper
{
    public static SongwriterViewModel ToViewModel(this SongwriterDTO songwriterDTO)
    {
        return new SongwriterViewModel() 
        {
            SongwriterId = songwriterDTO.Id,
            FullName = songwriterDTO.Name + " " + songwriterDTO.LastName,
            Compilations = new (songwriterDTO.CompilationDTOs.Select(e => e.ToViewModel())),
            Movies = new (songwriterDTO.MovieDTOs.Select(e => e.ToViewModel())),
        };
    }

    public static MovieViewModel ToViewModel(this MovieDTO movieDTO)
    {
        var entity = new MovieViewModel()
        {
            MovieId = movieDTO.Id,
            SongwriterId = movieDTO.SongwriterId,
            Director = movieDTO.Director?.ToViewModel(),
            ProductionCountry = movieDTO.ProductionCountry,
            ProductionYear = movieDTO.ProductionYear,
            Title = movieDTO.Title,
            MoviesReleasesLinks = new(movieDTO.MoviesReleasesLinks.Select(e => e.ToViewModel())),
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }

    public static MovieReleaseLinkViewModel ToViewModel(this MovieReleaseLinkDTO movieReleaseLinkDTO)
    {
        return new MovieReleaseLinkViewModel { IsFolder = movieReleaseLinkDTO.IsFolder, MovieRelease = movieReleaseLinkDTO.MovieReleaseDTO.ToViewModel() };
    }

    public static DirectorViewModel ToViewModel(this DirectorDTO directorDTO)
    {
        var entity = new DirectorViewModel()
        {
            Id = directorDTO.Id,
            FullName = directorDTO.FullName,
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }

    public static CompilationViewModel ToViewModel(this CompilationDTO compilationDTO)
    {
        var entity = new CompilationViewModel()
        {
            DiscId = compilationDTO.Id,
            Identifier = compilationDTO.Identifier,
            SongwriterId = compilationDTO.SongwriterId,
            ProductionCountry = compilationDTO.ProductionCountry,
            ProductionYear = compilationDTO.ProductionYear,
            SelectedDiscType = compilationDTO.DiscType,
            Songs = new(compilationDTO.SongDTOs.Select(e => e.ToViewModel())),
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }

    public static MovieReleaseViewModel ToViewModel(this MovieReleaseDTO movieReleaseLink)
    {
        var entity = new MovieReleaseViewModel()
        {
            DiscId = movieReleaseLink.Id,
            Identifier = movieReleaseLink.Identifier,
            ProductionCountry = movieReleaseLink.ProductionCountry,
            ProductionYear = movieReleaseLink.ProductionYear,
            SelectedDiscType = movieReleaseLink.DiscType,
            Songs = new (movieReleaseLink.SongDTOs.Select(e => e.ToViewModel())),
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }

    public static SongViewModel ToViewModel(this SongDTO songDTO)
    {
        var entity = new SongViewModel() 
        { 
            SongId = songDTO.Id,
            DiscId= songDTO.DiscId,
            Number = songDTO.SongNumber,
            DiscNumber = songDTO.DiscNumber,
            Title = songDTO.Name,
            Type = songDTO.ExecutableType.ToString(),
            Duration = songDTO.Duration,
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }
}
