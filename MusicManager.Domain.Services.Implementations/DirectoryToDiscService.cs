using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services
{
    public partial class DirectoryToDiscService : IPathToDiscService
    {
        private readonly string _bootLegKeyWord = "Bootleg";

        public Task<Result<Disc>> GetEntityAsync(string discPath, SongwriterId parent)
        {
            var result = isAbleToMoveNext(discPath);
            if (result.IsFailure)
            {
                return Task.FromResult(Result.Failure<Disc>(result.Error));
            }

            var directoryInfo = result.Value;
            if (directoryInfo.Name.Contains(_bootLegKeyWord))
            {
                return Task.FromResult(CreateBootLeg(directoryInfo, parent));
            }
            else
            {
                return Task.FromResult(CreateDisc(directoryInfo, parent));
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

        private Result<Disc> CreateBootLeg(DirectoryInfo discDirectoryInfo, SongwriterId parent)
        {
            var diskCreationResult = Disc.Create(
                parent,
                DiscType.Bootleg,
                discDirectoryInfo.Name,
                discDirectoryInfo.FullName);

            if (diskCreationResult.IsFailure)
            {
                return Result.Failure<Disc>(diskCreationResult.Error);
            }

            return diskCreationResult.Value;
        }

        private Result<Disc> CreateDisc(DirectoryInfo discDirectoryInfo, SongwriterId parent)
        {
            var gettingComponentsResult = GetDiscComponents(discDirectoryInfo.Name);

            if (gettingComponentsResult.IsFailure)
            {
                return Result.Failure<Disc>(gettingComponentsResult.Error);
            }

            var creationDiscResult = Disc.Create(
                parent,
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
}


