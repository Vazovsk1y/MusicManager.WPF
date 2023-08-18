using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;

namespace MusicManager.Domain.Services.Implementations;

public class SongwriterToFolderService : ISongwriterToFolderService
{
    private readonly IApplicationDbContext _dBcontext;
    private readonly IRoot _root;

    public SongwriterToFolderService(
        IApplicationDbContext dBcontext,
        IRoot root)
    {
        _dBcontext = dBcontext;
        _root = root;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Songwriter songwriter)
    {
        var rootDirectory = new DirectoryInfo(_root.RootPath);
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(new Error($"Root folder for songwriter is not exists {_root.RootPath}."));
        }

        string createdSongwriterDirectoryName = $"{songwriter.Name}{DomainServicesConstants.SongwriterDirectoryNameSeparator}{songwriter.Surname}";
        string createdSongwriterDirectoryFullPath = _root.CombineWith(createdSongwriterDirectoryName);
        string createdSongwriterRelationalPath = createdSongwriterDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdSongwriterDirectoryFullPath)
            || await _dBcontext.Songwriters.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdSongwriterRelationalPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this songwriter is already exists or songwriter with that directory info is already added to database."));
        }

        var createdSongwriterDirInfo = rootDirectory.CreateSubdirectory(createdSongwriterDirectoryName);
        createdSongwriterDirInfo.CreateSubdirectory(DomainServicesConstants.MOVIES_FOLDER_NAME);
        createdSongwriterDirInfo.CreateSubdirectory(DomainServicesConstants.COMPILATIONS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdSongwriterDirectoryFullPath, SongwriterEntityJson.FileName);
        await songwriter
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return Result.Success(createdSongwriterRelationalPath);
    }
}
