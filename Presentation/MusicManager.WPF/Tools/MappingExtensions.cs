using MusicManager.Domain.Enums;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.WPF.ViewModels.Entities;
using System.Linq;

namespace MusicManager.WPF.Tools;

internal static class MappingExtensions
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
            DirectorName = movieDTO.DirectorName,
            DirectorLastName = movieDTO.DirectorLastName,
            ProductionCountry = movieDTO.ProductionCountry,
            ProductionYear = movieDTO.ProductionYear,
            Title = movieDTO.Title,
            MoviesReleases = new(movieDTO.MovieReleasesDTOs.Select(e => e.ToViewModel())),
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

    public static MovieReleaseViewModel ToViewModel(this MovieReleaseDTO movieReleaseDTO)
    {
        var entity = new MovieReleaseViewModel()
        {
            DiscId = movieReleaseDTO.Id,
            Identifier = movieReleaseDTO.Identifier,
            ProductionCountry = movieReleaseDTO.ProductionCountry,
            ProductionYear = movieReleaseDTO.ProductionYear,
            SelectedDiscType = movieReleaseDTO.DiscType,
            Songs = new (movieReleaseDTO.SongDTOs.Select(e => e.ToViewModel())),
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
        };

        entity.SetCurrentAsPrevious();
        return entity;
    }
}
