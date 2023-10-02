using MusicManager.Domain.Shared;
using System.IO;

namespace MusicManager.Services.Implementations;

public class FileManagerInteractor : IFileManagerInteractor
{
    public Result<DirectoryInfo> SelectDirectory(string description = "Select a directory:")
    {
        using var folderBrowserDialog = new FolderBrowserDialog()
        {
            ShowNewFolderButton = false,
            Description = description,
        };

        var dialogResult = folderBrowserDialog.ShowDialog();
        if (dialogResult is DialogResult.OK)
        {
            return new DirectoryInfo(folderBrowserDialog.SelectedPath);
        }

        return Result.Failure<DirectoryInfo>(new Error("No directory was selected."));
    }

    public Result<FileInfo> SelectFile(string filter = "All files (*.*)|*.*", string title = "Choose file(s): ")
    {
        var fileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = filter,
            Title = title,
        };

        var dialogResult = fileDialog.ShowDialog();

        if (dialogResult is bool result && result is true)
        {
            return new FileInfo(fileDialog.FileName);
        }

        return Result.Failure<FileInfo>(new Error("No file was selected."));
    }
}
