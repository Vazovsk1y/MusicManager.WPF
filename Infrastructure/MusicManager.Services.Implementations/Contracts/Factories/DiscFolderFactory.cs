using MusicManager.Domain.Constants;
using MusicManager.Domain.Services;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.IO;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class DiscFolderFactory : IDiscFolderFactory
{
    private readonly string[] _allowedFilesExtensions = new[]
    {
        DomainConstants.WVExtension,
        DomainConstants.Mp3Extension,
        DomainConstants.ApeExtension,
        DomainConstants.FlacExtension,
        DomainConstants.CueExtension,
    };

    private readonly ISongFileFactory _songFileFactory;

    public DiscFolderFactory(ISongFileFactory songFileFactory)
    {
        _songFileFactory = songFileFactory;
    }

    public Result<DiscFolder> Create(DirectoryInfo discDirectory)
    {
        if (!discDirectory.Exists)
        {
            return Result.Failure<DiscFolder>(DomainServicesErrors.PassedDirectoryIsNotExists(discDirectory.FullName));
        }

        var coversFolder = discDirectory
            .EnumerateDirectories()
            .FirstOrDefault(d => d.Name == DomainServicesConstants.COVERS_FOLDER_NAME);

        List<string> covers = coversFolder is null ? new List<string>() : coversFolder.EnumerateFiles().Select(f => f.FullName).ToList();
        var folderJPGfile = discDirectory
            .EnumerateFiles()
            .FirstOrDefault(f => f.Name == DomainServicesConstants.FolderJPG);

        if (folderJPGfile is not null)
        {
            covers.Add(folderJPGfile.FullName);
        }

        List<SongFile> songsFiles = new();
        var songsFromCdFolders = discDirectory
            .EnumerateDirectories()
            .Where(d => d.Name.StartsWith(DiscNumber.CD_KEYWORD))
            .SelectMany(d => 
            d.EnumerateFiles().Where(f => _allowedFilesExtensions.Contains(f.Extension)));

        var allSongsFiles = songsFromCdFolders
            .Union(discDirectory.EnumerateFiles().Where(f => _allowedFilesExtensions.Contains(f.Extension))).ToList();

        var cueFiles = allSongsFiles.Where(f => f.Extension == DomainConstants.CueExtension);

        foreach (var cueFile in cueFiles)
        {
            var executableFileForCue = allSongsFiles
                .FirstOrDefault(e => e.Name.Contains(Path.GetFileNameWithoutExtension(cueFile.Name)) && e.Extension != DomainConstants.CueExtension);

            if (executableFileForCue is null)
            {
                return Result.Failure<DiscFolder>(new($"Couldn't find an executable file for cue file [{cueFile.FullName}]."));
            }

            var result = _songFileFactory.Create(cueFile);
            if (result.IsFailure)
            {
                return Result.Failure<DiscFolder>(result.Error);
            }

            allSongsFiles.Remove(executableFileForCue);
            allSongsFiles.Remove(cueFile);
            songsFiles.Add(result.Value);
        }

        foreach (var songFile in allSongsFiles)
        {
            var result = _songFileFactory.Create(songFile);
            if (result.IsFailure)
            {
                return Result.Failure<DiscFolder>(result.Error);
            }

            songsFiles.Add(result.Value);
        }

        return new DiscFolder(discDirectory.FullName, songsFiles, covers);
    }
}
