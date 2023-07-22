using MusicManager.Domain.Shared;

namespace MusicManager.Services;

public interface IFileManagerInteractor
{
    Result<DirectoryInfo> GetSelectedDirectory(string description = "Select a directory:");

    Result<FileInfo> GetSelectedFile();
}
