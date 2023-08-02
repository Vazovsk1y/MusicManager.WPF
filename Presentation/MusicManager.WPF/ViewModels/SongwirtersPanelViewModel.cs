using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Utils;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicManager.WPF.ViewModels;

internal class SongwirtersPanelViewModel : ObservableRecipient
{
    private readonly ObservableCollection<SongwriterViewModel> _songwriters = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFileManagerInteractor _fileManagerInteractor;
    private readonly ISongwriterFolderFactory _songwriterFolderFactory;

    public SongwirtersPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }
    
    public SongwirtersPanelViewModel(
        IServiceScopeFactory serviceScopeFactory,
        IFileManagerInteractor fileManagerInteractor,
        ISongwriterFolderFactory songwriterFolderFactory)
    {

        _serviceScopeFactory = serviceScopeFactory;
        _fileManagerInteractor = fileManagerInteractor;
        _songwriterFolderFactory = songwriterFolderFactory;
    }

    private SongwriterViewModel? _selectedSongwriter;

    public ObservableCollection<SongwriterViewModel> Songwriters => _songwriters;

    public SongwriterViewModel? SelectedSongwriter
    {
        get => _selectedSongwriter;
        set => SetProperty(ref _selectedSongwriter, value);
    }

    #region --Commands--

    private IAsyncRelayCommand _addSongwriterFromFolderCommand;

    public IAsyncRelayCommand AddSongwriterFromFolderCommand => _addSongwriterFromFolderCommand ??=
        new AsyncRelayCommand(OnSongwriterAddFromFolderExecute, () => !AddSongwriterFromFolderCommand.IsRunning);

    private async Task OnSongwriterAddFromFolderExecute()
    {
        var selectedFolderResult = _fileManagerInteractor.SelectDirectory();
        if (selectedFolderResult.IsFailure)
        {
            MessageBox.Show(selectedFolderResult.Error.Message);
            return;
        }

        var creatingSongwriterFolderResult = _songwriterFolderFactory.Create(selectedFolderResult.Value);
        if (creatingSongwriterFolderResult.IsFailure)
        {
            MessageBox.Show(creatingSongwriterFolderResult.Error.Message);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var songwriterService = scope.ServiceProvider.GetRequiredService<ISongwriterService>();

        var addingResult = await songwriterService.SaveFromFolderAsync(creatingSongwriterFolderResult.Value);
        if (addingResult.IsFailure)
        {
            MessageBox.Show(addingResult.Error.Message);
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Songwriters.Add(addingResult.Value.ToViewModel());
        });
        MessageBox.Show("Success");
    }

    #endregion

    protected override async void OnActivated()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var songwriterService = scope.ServiceProvider.GetRequiredService<ISongwriterService>();

        var result = await songwriterService.GetAllAsync();
        if (result.IsSuccess)
        {
            foreach (var songwriterDTO in result.Value)
            {
                Songwriters.Add(songwriterDTO.ToViewModel());
            }
        }
    }
}

