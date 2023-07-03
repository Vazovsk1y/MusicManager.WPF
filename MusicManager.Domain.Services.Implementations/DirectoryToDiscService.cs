using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Services.Implementations.Extensions;
using MusicManager.Domain.Shared;
using System.Text;

namespace MusicManager.Domain.Services
{
    public class DirectoryToDiscService : IPathToDiscService
    {
        private readonly char _diskProductionInfoSeparator = '-';
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

        private Result<DirectoryInfo> isAbleToMoveNext(string path)
        {
            if (!PathValidator.IsValid(path))
            {
                return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryPathIsInvalid(path));
            }

            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryIsNotExists(path));
            }
            return directoryInfo;
        }

        private Result<Disc> CreateBootLeg(DirectoryInfo disc, SongwriterId parent)
        {
            string identifier = disc.Name.Trim(_bootLegKeyWord.ToCharArray());
            var diskCreationResult = Disc.Create(
                parent,
                DiscType.Bootleg,
                identifier.Length > 0 ? identifier : DiscType.Bootleg.MapToString(),
                disc.FullName);

            if (diskCreationResult.IsFailure)
            {
                return Result.Failure<Disc>(diskCreationResult.Error);
            }

            return diskCreationResult.Value;
        }

        private Result<Disc> CreateDisc(DirectoryInfo disc, SongwriterId parent)
        {
            var diskTypeRow = disc.Name[..disc.Name.IndexOf(' ')];
            var indetificator = GetIndetificator(disc.Name, diskTypeRow.Length);
            var (isSuccess, year, country) = GetProductionInfoComponents(disc.Name, indetificator.Length, diskTypeRow.Length);

            if (!isSuccess)
            {
                return Result.Failure<Disc>(new Error("Error occured when tried to get production inforamation."));
            }

            var creationDiskTypeResult = diskTypeRow.CreateDiscType();
            if (creationDiskTypeResult.IsFailure)
            {
                return Result.Failure<Disc>(creationDiskTypeResult.Error);
            }

            var creationDiscResult = Disc.Create(
                parent,
                creationDiskTypeResult.Value,
                indetificator.RemoveAllSpaces(),
                disc.FullName,
                year!,
                country!);

            if (creationDiscResult.IsFailure)
            {
                return creationDiscResult;
            }

            return Result.Success(creationDiscResult.Value);
        }

        private (bool isSuccess, string? year, string? country) GetProductionInfoComponents(string directoryName, int indetificatorLength, int diskTypeRowLength)
        {
            var productionComponents = new string(directoryName.Skip(indetificatorLength + diskTypeRowLength).ToArray())
                .Split(_diskProductionInfoSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.TrimStart().TrimEnd())
                .ToList();

            if (productionComponents.Count < 2)
            {
                return (false, null, null);
            }

            return (true, productionComponents[1], productionComponents[0]);
        }

        private string GetIndetificator(string directoryName, int diskTypeRowLength)
        {
            StringBuilder indetificator = new();
            for (int i = diskTypeRowLength; i < directoryName.Length; i++)
            {
                if (directoryName[i] == '-')
                    break;
                indetificator.Append(directoryName[i]);
            }

            return indetificator.ToString();
        }
    }
}


