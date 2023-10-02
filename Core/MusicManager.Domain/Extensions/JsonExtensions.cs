﻿using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using System.Text.Json;

namespace MusicManager.Domain.Extensions;

public static class JsonExtensions
{
    public static Result<Songwriter> ToEntity(this SongwriterEntityJson entityJson, string associatedDirectoryPath)
    {
        return Songwriter.Create(
            entityJson.Name, 
            entityJson.LastName, 
            associatedDirectoryPath);
    }

    public static Result<Compilation> ToEntity(this CompilationEntityJson entityJson, SongwriterId songwriterId, string associatedDirectoryPath)
    {
        return Compilation.Create(
            songwriterId,
            DiscType.Create(entityJson.DiscType).Value,
            entityJson.Identifier,
            associatedDirectoryPath,
            entityJson.ProductionYear,
            entityJson.ProductionCountry
            );
    }

    public static Result<Movie> ToEntity(this MovieEntityJson entityJson, SongwriterId songwriterId, string associatedDirectoryPath)
    {
        return Movie.Create(
            songwriterId,
            entityJson.Title,
            associatedDirectoryPath,
            entityJson.ProductionYear,
            entityJson.ProductionCountry
            );
    }

    public static Result<MovieRelease> ToEntity(this MovieReleaseEntityJson entityJson, string associatedDirectoryPath)
    {
        return MovieRelease.Create(
            DiscType.Create(entityJson.DiscType).Value,
            entityJson.Identifier,
            associatedDirectoryPath,
            entityJson.ProductionYear,
            entityJson.ProductionCountry
            );
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
            ProductionYear = (int)movie.ProductionInfo.Year!,
            ProductionCountry = movie.ProductionInfo.Country,
        };
    }

    public static CompilationEntityJson ToJson(this Compilation compilation)
    {
        return new CompilationEntityJson
        {
            Identifier = compilation.Identifier,
            DiscType = compilation.Type.Value,
            ProductionCountry = compilation.ProductionInfo.Country,
            ProductionYear = compilation.ProductionInfo.Year,
        };
    }

    public static MovieReleaseEntityJson ToJson(this MovieRelease movieRelease)
    {
        return new MovieReleaseEntityJson
        {
            Identifier = movieRelease.Identifier,
            DiscType = movieRelease.Type.Value,
            ProductionCountry = movieRelease.ProductionInfo.Country,
            ProductionYear = movieRelease.ProductionInfo.Year,
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
