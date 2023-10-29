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
    private static readonly string[] _allowedFilesExtensions = new[]
    {
        DomainConstants.WVExtension,
        DomainConstants.Mp3Extension,
        DomainConstants.ApeExtension,
        DomainConstants.FlacExtension,
    };

    private readonly ISongFileFactory _songFileFactory;
    private readonly ICueFileInteractor _cueFileInteractor;

    public DiscFolderFactory(
		ISongFileFactory songFileFactory, 
		ICueFileInteractor cueFileInteractor)
    {
        _songFileFactory = songFileFactory;
        _cueFileInteractor = cueFileInteractor;
    }

    public Result<DiscFolder> Create(DirectoryInfo discDirectory, string? linkPath = null)
    {
        if (!discDirectory.Exists)
        {
            return Result.Failure<DiscFolder>(DomainServicesErrors.PassedDirectoryIsNotExists(discDirectory.FullName));
        }

        var coversResult = GetCoversResult(discDirectory);
        if (coversResult.IsFailure)
        {
            return Result.Failure<DiscFolder>(coversResult.Error);
        }

		var songsResult = GetSongsResult(discDirectory);
		if (songsResult.IsFailure)
		{
			return Result.Failure<DiscFolder>(songsResult.Error);
		}

        return new DiscFolder(
			discDirectory.FullName, 
			songsResult.Value, 
			coversResult.Value, 
			linkPath);
    }

	private Result<IReadOnlyCollection<SongFile>> GetSongsResult(DirectoryInfo discDirectory)
	{
		List<SongFile> result = new();
		var allFiles = discDirectory.EnumerateDirectories()
			.Where(d => d.Name.StartsWith(DiscNumber.CD_KEYWORD))
			.SelectMany(d => d.EnumerateFiles())
			.Union(discDirectory.EnumerateFiles());

		var audioFilesInfo = allFiles
			.Where(f => _allowedFilesExtensions.Contains(f.Extension))
			.ToList();

		var cueFiles = allFiles
			.Where(f => string.Equals(f.Extension, DomainConstants.CueExtension, StringComparison.OrdinalIgnoreCase));

		foreach (var cueFile in cueFiles)
		{
			var cueInteractorResult = _cueFileInteractor.GetCueSheet(cueFile.FullName);
			if (cueInteractorResult.IsFailure)
			{
				return Result.Failure<IReadOnlyCollection<SongFile>>(cueInteractorResult.Error);
			}

			var executableFileForCue = audioFilesInfo
				.FirstOrDefault(e => e.Name == cueInteractorResult.Value.ExecutableFileName);

			if (executableFileForCue is null)
			{
				return Result.Failure<IReadOnlyCollection<SongFile>>(new($"Couldn't find an executable file for cue file [{cueFile.FullName}]."));
			}

			var songFileCreationResult = _songFileFactory.Create(cueFile);
			if (songFileCreationResult.IsFailure)
			{
				return Result.Failure<IReadOnlyCollection<SongFile>>(songFileCreationResult.Error);
			}

			audioFilesInfo.Remove(executableFileForCue);
			result.Add(songFileCreationResult.Value);
		}

		foreach (var songFile in audioFilesInfo)
		{
			var songFileResult = _songFileFactory.Create(songFile);
			if (songFileResult.IsFailure)
			{
				return Result.Failure<IReadOnlyCollection<SongFile>>(songFileResult.Error);
			}
			result.Add(songFileResult.Value);
		}

		return result;
	}

	private static Result<IReadOnlyCollection<string>> GetCoversResult(DirectoryInfo discDirectory)
    {
		var coversFolder = discDirectory
			.EnumerateDirectories()
			.FirstOrDefault(d => string.Equals(d.Name, DomainServicesConstants.COVERS_FOLDER_NAME, StringComparison.OrdinalIgnoreCase));

		List<string> result = coversFolder is null ? new List<string>() : coversFolder.EnumerateFiles().Select(f => f.FullName).ToList();
		var folderJPGfile = discDirectory
			.EnumerateFiles()
			.FirstOrDefault(f => string.Equals(f.Name, DomainServicesConstants.FolderJPGFileName, StringComparison.OrdinalIgnoreCase));

		if (folderJPGfile is not null)
		{
			result.Add(folderJPGfile.FullName);
		}

        return result;
	}
}
