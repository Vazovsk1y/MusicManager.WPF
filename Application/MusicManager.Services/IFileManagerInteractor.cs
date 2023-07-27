using MusicManager.Domain.Shared;

namespace MusicManager.Services;

public interface IFileManagerInteractor
{
    Result<DirectoryInfo> SelectDirectory(string description = "Select a directory:");

    Result<FileInfo> SelectFile();
}
