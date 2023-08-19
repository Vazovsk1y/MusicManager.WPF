using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.Services.Contracts;
using MusicManager.WPF.Tools;

namespace MusicManager.WPF.ViewModels;

internal partial class SettingsViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _rootPath;

    [ObservableProperty]
    private bool _deleteAssociatedFolder;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFileManagerInteractor _fileManagerInteractor;


    public SettingsViewModel(
        IServiceScopeFactory serviceScopeFactory, 
        IFileManagerInteractor fileManagerInteractor)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _fileManagerInteractor = fileManagerInteractor;
    }

    [RelayCommand]
    private void Save()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IAppConfig>();
        config.RootPath = RootPath;
        config.DeleteAssociatedFolder = DeleteAssociatedFolder;
        config.Save();
    }

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
        using var scope = _serviceScopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IAppConfig>();
        RootPath = config.RootPath;
        DeleteAssociatedFolder = config.DeleteAssociatedFolder;
    }
}
