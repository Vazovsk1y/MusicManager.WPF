using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class SongToFileService : ISongToFileService
{
    private readonly IRoot _root;

    public SongToFileService(
        IRoot root)
    {
        _root = root;
    }

    public Task<Result<string>> CopyToAsync(string fileFullPath, Disc parent, DiscNumber? discNumber = null, CancellationToken cancellationToken = default)
    {
        if (parent.AssociatedFolderInfo is null)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Parent directory info isn't setted, unable to copy files.")));
        }

        var rootDirectory = new DirectoryInfo(_root.CombineWith(parent.AssociatedFolderInfo.Path));
        if (!rootDirectory.Exists)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Parent directory info isn't exists, unable to copy files.")));
        }

        var currentFileLocation = new FileInfo(_root.CombineWith(fileFullPath));
        if (!currentFileLocation.Exists)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Passed file isn't exists, unable to copy that.")));
        }

        bool isFileAlreadyExists = rootDirectory
            .EnumerateDirectories()
            .Where(e => e.Name.StartsWith(DiscNumber.CD_KEYWORD))
            .SelectMany(e => e.EnumerateFiles())
            .Union(rootDirectory.EnumerateFiles())
            .FirstOrDefault(e => e.Name == currentFileLocation.Name) is not null;

        if (isFileAlreadyExists)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Passed file is already exists, unable to copy that again.")));
        }

        string copyFilePath;
        if (discNumber is not null)
        {
            var root = DirectoryHelper.CreateIfNotExists(Path.Combine(rootDirectory.FullName, discNumber.Value));
            copyFilePath = Path.Combine(root.FullName, currentFileLocation.Name);
        }
        else
        {
            copyFilePath = Path.Combine(rootDirectory.FullName, currentFileLocation.Name);
        }

        var newFileLocation = currentFileLocation.CopyTo(copyFilePath);
        return Task.FromResult(Result.Success(newFileLocation.FullName.GetRelational(_root)));
    }

    public Task<Result<string>> UpdateAsync(Song song, CancellationToken cancellationToken = default)
    {
        if (song.IsFromCue)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Unable to modify the songFile from cue.")));
        }

        var previousFile = new FileInfo(_root.CombineWith(song.PlaybackInfo.ExecutableFilePath));
        if (!previousFile.Exists)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Executable song file is not exists.")));
        }

        string newFileName = $"{song.Order} {song.Title}{previousFile.Extension}";
        string newFileFullPath = Path.Combine(previousFile.DirectoryName!, newFileName);

        if (previousFile.Name != newFileName)
        {
            previousFile.MoveTo(newFileFullPath);
        }

        using var songFileInfo = TagLib.File.Create(newFileFullPath);
        songFileInfo.Tag.Title = song.Title;
        songFileInfo.Tag.Track = (uint)song.Order;
        songFileInfo.Save();

        return Task.FromResult(Result.Success(newFileFullPath.GetRelational(_root)));
    }
}
