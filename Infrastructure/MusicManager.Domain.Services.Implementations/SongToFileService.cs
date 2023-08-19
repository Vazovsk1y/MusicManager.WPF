using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
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

    public Task<Result<string>> CopyToAsync(string fileFullPath, Disc parent, DiscNumber? discNumber = null)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Parent directory info isn't setted, unable to copy files.")));
        }

        var rootDirectory = new DirectoryInfo(_root.CombineWith(parent.EntityDirectoryInfo.Path));
        if (!rootDirectory.Exists)
        {
            return Task.FromResult(Result.Failure<string>(new Error("Parent directory info isn't exists, unable to copy files.")));
        }

        var currentFileLocation = new FileInfo(fileFullPath);
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
}
