using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using System.Text.Json;

namespace MusicManager.Domain.Extensions
{
    public static class JsonExtensions
    {
        public static Result<Songwriter> ToEntity(this SongwriterEntityJson entityJson, string associatedDirectoryPath)
        {
            return Songwriter.Create(entityJson.Name, entityJson.LastName, associatedDirectoryPath);
        }

        public static Result<Compilation> ToEntity(this CompilationEntityJson entityJson, SongwriterId songwriterId, string associatedDirectoryPath)
        {
            if (entityJson.ProductionCountry is null || entityJson.ProductionYear is null)
            {
                return Compilation.Create(songwriterId, entityJson.DiscType, entityJson.Identifier, associatedDirectoryPath);
            }

            return Compilation.Create(
                songwriterId, 
                entityJson.DiscType, 
                entityJson.Identifier, 
                associatedDirectoryPath,
                (int)entityJson.ProductionYear, 
                entityJson.ProductionCountry);
        }

        public static Result<Movie> ToEntity(this MovieEntityJson entityJson, SongwriterId songwriterId, string associatedDirectoryPath)
        {
            if (entityJson.ProductionCountry is null || entityJson.ProductionYear is null)
            {
                return Movie.Create(songwriterId, entityJson.Title, associatedDirectoryPath);
            }

            if (entityJson.DirectorLastName is null || entityJson.DirectorName is null)
            {
                return Movie.Create(songwriterId, entityJson.Title, (int)entityJson.ProductionYear, entityJson.ProductionCountry, associatedDirectoryPath);
            }

            return Movie.Create(
                songwriterId,
                entityJson.Title,
                (int)entityJson.ProductionYear,
                entityJson.ProductionCountry,
                associatedDirectoryPath,
                entityJson.DirectorName,
                entityJson.DirectorLastName);
        }

        public static Result<MovieRelease> ToEntity(this MovieReleaseEntityJson entityJson, string associatedDirectoryPath)
        {
            if (entityJson.ProductionCountry is null || entityJson.ProductionYear is null)
            {
                return MovieRelease.Create(entityJson.DiscType, entityJson.Identifier, associatedDirectoryPath);
            }

            return MovieRelease.Create(
                entityJson.DiscType,
                entityJson.Identifier,
                associatedDirectoryPath,
                (int)entityJson.ProductionYear,
                entityJson.ProductionCountry);
        }

        public static SongwriterEntityJson ToJson(this Songwriter songwriter)
        {
            return new SongwriterEntityJson
            {
                Name = songwriter.Name,
                LastName = songwriter.Surname,
            };
        }

        public static MovieEntityJson ToJson(this Movie movie)
        {
            return new MovieEntityJson
            {
                Title = movie.Title,
                ProductionYear = movie.ProductionInfo?.Year,
                ProductionCountry = movie.ProductionInfo?.Country,
                DirectorName = movie.DirectorInfo?.Name,
                DirectorLastName = movie.DirectorInfo?.Surname
            };
        }

        public static CompilationEntityJson ToJson(this Compilation compilation)
        {
            return new CompilationEntityJson
            {
                Identifier = compilation.Identifier,
                DiscType = compilation.Type,
                ProductionCountry = compilation.ProductionInfo?.Country,
                ProductionYear = compilation.ProductionInfo?.Year,
            };
        }

        public static MovieReleaseEntityJson ToJson(this MovieRelease movieRelease)
        {
            return new MovieReleaseEntityJson
            {
                Identifier = movieRelease.Identifier,
                DiscType = movieRelease.Type,
                ProductionCountry = movieRelease.ProductionInfo?.Country,
                ProductionYear = movieRelease.ProductionInfo?.Year,
            };
        }

        public static async Task AddSerializedJsonEntityToAsync<TEntity>(this SerializableEntity<TEntity> entity, string path)
        where TEntity : class, IAggregateRoot
        {
            using var writer = new StreamWriter(path);
            string json = JsonSerializer.Serialize(entity, entity.GetType());
            await writer.WriteAsync(json);
        }
    }
}
