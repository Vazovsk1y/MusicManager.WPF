using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.WPF.Infrastructure;
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

    private readonly IFileSystemManager _fileManagerInteractor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

	public UserConfigViewModel(
		IFileSystemManager fileManagerInteractor, IServiceScopeFactory serviceScopeFactory)
	{
		_fileManagerInteractor = fileManagerInteractor;
		_serviceScopeFactory = serviceScopeFactory;
	}

	[RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
		using var scope = _serviceScopeFactory.CreateScope();
		var config = scope.ServiceProvider.GetRequiredService<IUserConfig>();
        config.CreateAssociatedFolder = CreateAssociatedFolder;
        config.Path = RootPath;
        config.Save();
	}

    private bool CanSave() => RootPath is not null;

	[RelayCommand]
    private void SelectRoot()
    {
        var selectedFolderResult = _fileManagerInteractor.SelectFolder();
        if (selectedFolderResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(selectedFolderResult.Error.Message);
            return;
        }

        RootPath = selectedFolderResult.Value.FullName;
    }

    protected override void OnActivated()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IUserConfig>();
        RootPath = config.Path;
        CreateAssociatedFolder = config.CreateAssociatedFolder;
    }
}
