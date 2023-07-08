using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations;

public partial class DirectoryToMovieReleaseService : IPathToMovieReleaseService
{
    private readonly string _bootLegKeyWord = "Bootleg";

    public Task<Result<MovieRelease>> GetEntityAsync(string compilationPath)
    {
        var result = isAbleToMoveNext(compilationPath);
        if (result.IsFailure)
        {
            return Task.FromResult(Result.Failure<MovieRelease>(result.Error));
        }

        var directoryInfo = result.Value;
        if (directoryInfo.Name.Contains(_bootLegKeyWord))
        {
            return Task.FromResult(CreateBootLeg(directoryInfo));
        }
        else
        {
            return Task.FromResult(CreateDisc(directoryInfo));
        }
    }

    private Result<DirectoryInfo> isAbleToMoveNext(string discPath)
    {
        if (!PathValidator.IsValid(discPath))
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryPathIsInvalid(discPath));
        }

        var directoryInfo = new DirectoryInfo(discPath);
        if (!directoryInfo.Exists)
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryIsNotExists(discPath));
        }
        return directoryInfo;
    }

    private Result<MovieRelease> CreateBootLeg(DirectoryInfo discDirectoryInfo)
    {
        var diskCreationResult = MovieRelease.Create(
            DiscType.Bootleg,
            discDirectoryInfo.Name,
            discDirectoryInfo.FullName);

        if (diskCreationResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(diskCreationResult.Error);
        }

        return diskCreationResult.Value;
    }

    private Result<MovieRelease> CreateDisc(DirectoryInfo discDirectoryInfo)
    {
        var gettingComponentsResult = GetDiscComponents(discDirectoryInfo.Name);

        if (gettingComponentsResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(gettingComponentsResult.Error);
        }

        var creationDiscResult = MovieRelease.Create(
            gettingComponentsResult.Value.discType,
            gettingComponentsResult.Value.discIndetificator,
            discDirectoryInfo.FullName,
            gettingComponentsResult.Value.year,
            gettingComponentsResult.Value.country);

        if (creationDiscResult.IsFailure)
        {
            return creationDiscResult;
        }

        return Result.Success(creationDiscResult.Value);
    }

    private Result<(DiscType discType, string discIndetificator, string country, string year)> GetDiscComponents(string discDirectoryName)
    {
        var match = FindAllDiscComponents().Match(discDirectoryName);
        if (match.Success)
        {
            var discTypeCreationResult = match.Groups[1].Value.CreateDiscType();
            if (discTypeCreationResult.IsFailure)
            {
                return Result.Failure<(DiscType, string, string, string)>(discTypeCreationResult.Error);
            }

            return (discTypeCreationResult.Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);
        }

        return Result.Failure<(DiscType, string, string, string)>(new Error($"Unable to get some of the required components from disc directory name [{discDirectoryName}]."));
    }

    [GeneratedRegex(@"^(.*?)\s(.*?)\s-\s(.*?)\s-\s(\d{4})")]
    private partial Regex FindAllDiscComponents();
}
