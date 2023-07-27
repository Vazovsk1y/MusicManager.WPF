using MusicManager.Domain.Shared;
using System.Windows.Forms;

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

    public Result<FileInfo> SelectFile()
    {
        throw new NotImplementedException();
    }
}
