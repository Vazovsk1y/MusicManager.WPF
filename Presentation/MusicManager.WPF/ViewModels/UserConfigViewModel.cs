using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using MusicManager.WPF.Tools;
using System.IO;
using System.Text.Json;

namespace MusicManager.WPF.ViewModels;

internal partial class UserConfigViewModel : 
    ObservableRecipient
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _rootPath;

    [ObservableProperty]
    private bool _createAssociatedFolder;

    private readonly IFileManagerInteractor _fileManagerInteractor;

    public UserConfig CurrentConfig => new()
	{
		CreateAssociatedFolder = CreateAssociatedFolder,
		RootPath = RootPath,
	};

	public UserConfigViewModel(
        IFileManagerInteractor fileManagerInteractor)
    {
        _fileManagerInteractor = fileManagerInteractor;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var settingsModel = new UserConfig
        {
            CreateAssociatedFolder = CreateAssociatedFolder,
            RootPath = RootPath,
        };
        settingsModel.Save();
	}

    private bool CanSave() => RootPath is not null;

	[RelayCommand]
    private void SelectRoot()
    {
        var selectedFolderResult = _fileManagerInteractor.SelectDirectory();
        if (selectedFolderResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(selectedFolderResult.Error.Message);
            return;
        }

        RootPath = selectedFolderResult.Value.FullName;
    }

    protected override void OnActivated()
    {
        var fileInfo = new FileInfo(UserConfig.SettingsFileFullPath);
        if (!fileInfo.Exists)
        {
            RootPath = UserConfig.Default.RootPath;
            CreateAssociatedFolder = UserConfig.Default.CreateAssociatedFolder;
            return;
        }

        using var stream = fileInfo.OpenRead();
        var config = JsonSerializer.Deserialize<UserConfig>(stream) ?? UserConfig.Default;   
        RootPath = config.RootPath;
        CreateAssociatedFolder = config.CreateAssociatedFolder;
    }
}
