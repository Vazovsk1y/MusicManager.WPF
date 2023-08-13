using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class SongwriterToFolderService : ISongwriterToFolderService
{
    private readonly IApplicationDbContext _dBcontext;
    private readonly string _rootNameIfSongwritersCountZero;

    public SongwriterToFolderService(
        IApplicationDbContext dBcontext,
        IHostEnvironment hostEnvironment)
    {
        _dBcontext = dBcontext;
        _rootNameIfSongwritersCountZero = $"{hostEnvironment.ApplicationName} Entities";
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Songwriter songwriter)
    {
        var rootPathResult = await GetRootPath();
        if (rootPathResult.IsFailure)
        {
            return rootPathResult;
        }

        string rootPath = rootPathResult.Value;
        var (isCreated, message) = DirectoryHelper.TryToCreateIfNotExists(rootPath, out var rootDirectory);
        if (!isCreated)
        {
            return Result.Failure<string>(new(message));
        }

        string createdSongwriterDirectoryName = $"{songwriter.Name}{DomainServicesConstants.SongwriterDirectoryNameSeparator}{songwriter.Surname}";
        string createdSongwriterDirectoryFullPath = Path.Combine(rootPath, createdSongwriterDirectoryName);

        if (Directory.Exists(createdSongwriterDirectoryFullPath)
            || await _dBcontext.Songwriters.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdSongwriterDirectoryFullPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this songwriter is already exists or songwriter with that directory info is already added to database."));
        }

        var createdSongwriterDirInfo = rootDirectory!.CreateSubdirectory(createdSongwriterDirectoryName);
        createdSongwriterDirInfo.CreateSubdirectory(DomainServicesConstants.MOVIES_FOLDER_NAME);
        createdSongwriterDirInfo.CreateSubdirectory(DomainServicesConstants.COMPILATIONS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdSongwriterDirectoryFullPath, SongwriterEntityJson.FileName);
        await songwriter
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return Result.Success(createdSongwriterDirectoryFullPath);
    }

    private async Task<Result<string>> GetRootPath()
    {
        var anyOtherSongwriter = await _dBcontext.Songwriters.FirstOrDefaultAsync(s => s.EntityDirectoryInfo != null);

        if (anyOtherSongwriter is not null)
        {
            var rootPath = Path.GetDirectoryName(anyOtherSongwriter!.EntityDirectoryInfo!.FullPath)
                ?? throw new InvalidOperationException("Must be named correct, so null return is not possible.");

            if (!Directory.Exists(rootPath))
            {
                return Result.Failure<string>(new Error($"Unable to find a root folder for songwriter {rootPath}"));
            }

            return rootPath;
        }
        else
        {
            return Path.Combine(DirectoryHelper.LocalApplicationDataPath, _rootNameIfSongwritersCountZero);
        }
    }
}
