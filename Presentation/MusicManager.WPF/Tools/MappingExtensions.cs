using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.ValueObjects;
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
        return new MovieViewModel()
        {
            MovieId = movieDTO.Id,
            SongwriterId = movieDTO.SongwriterId,
            DirectorFullName = movieDTO.DirectorName + " " + movieDTO.DirectorLastName,
            ProductionCountry = movieDTO.ProductionCountry ?? ProductionInfo.UndefinedCountry,
            ProductionYear = movieDTO.ProductionYear,
            Title = movieDTO.Title,
            MoviesReleases = new(movieDTO.MovieReleasesDTOs.Select(e => e.ToViewModel())),
        };
    }

    public static CompilationViewModel ToViewModel(this CompilationDTO compilationDTO)
    {
        return new CompilationViewModel()
        {
            DiscId = compilationDTO.Id,
            Identificator = compilationDTO.Identifier,
            SongwriterId = compilationDTO.SongwriterId,
            ProductionCountry = compilationDTO.ProductionCountry ?? ProductionInfo.UndefinedCountry,
            ProductionYear = compilationDTO.ProductionYear,
            DiscType = compilationDTO.DiscType.Value,
            Songs = new(compilationDTO.SongDTOs.Select(e => e.ToViewModel())),
        };
    }

    public static MovieReleaseViewModel ToViewModel(this MovieReleaseDTO movieReleaseDTO)
    {
        return new MovieReleaseViewModel()
        {
            DiscId = movieReleaseDTO.Id,
            MoviesLinks = movieReleaseDTO.MoviesLinks.ToList(),
            Identificator = movieReleaseDTO.Identifier,
            ProductionCountry = movieReleaseDTO.ProductionCountry ?? ProductionInfo.UndefinedCountry,
            ProductionYear = movieReleaseDTO.ProductionYear,
            DiscType = movieReleaseDTO.DiscType.Value,
            Songs = new (movieReleaseDTO.SongDTOs.Select(e => e.ToViewModel())),
        };
    }

    public static SongViewModel ToViewModel(this SongDTO songDTO)
    {
        return new SongViewModel() 
        { 
            SongId = songDTO.Id,
            DiscId= songDTO.DiscId,
            Number = songDTO.SongNumber,
            DiscNumber = songDTO.DiscNumber,
            Title = songDTO.Name,
            ExecutableType = songDTO.ExecutableType is null ? 
            SongFileType.Unknown.ToString() 
            : 
            songDTO.ExecutableType.ToString(),
        };
    }
}
