using MusicManager.Domain.Shared;

namespace MusicManager.Services;

public interface IFileSystemManager
{
    Result<DirectoryInfo> SelectFolder(string description = "Select a folder:");

    Result<FileInfo> SelectFile(string filter = "All files (*.*)|*.*", string title = "Choose file(s):");
}
