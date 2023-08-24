using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class SongwirtersPanelViewModel : 
    ObservableRecipient, 
    IRecipient<SongwriterCreatedMessage>
{
    private readonly ObservableCollection<SongwriterViewModel> _songwriters = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFileManagerInteractor _fileManagerInteractor;
    private readonly ISongwriterFolderFactory _songwriterFolderFactory;
    private readonly IUserDialogService<SongwriterAddWindow> _dialogService;

    public SongwirtersPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public SongwirtersPanelViewModel(
        IServiceScopeFactory serviceScopeFactory,
        IFileManagerInteractor fileManagerInteractor,
        ISongwriterFolderFactory songwriterFolderFactory,
        IUserDialogService<SongwriterAddWindow> dialogService) : base()
    {

        _serviceScopeFactory = serviceScopeFactory;
        _fileManagerInteractor = fileManagerInteractor;
        _songwriterFolderFactory = songwriterFolderFactory;
        _dialogService = dialogService;
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
        using var scope = _serviceScopeFactory.CreateScope();
        var selectedFolderResult = _fileManagerInteractor.SelectDirectory();
        if (selectedFolderResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(selectedFolderResult.Error.Message);
            return;
        }

        var creatingSongwriterFolderResult = _songwriterFolderFactory.Create(selectedFolderResult.Value);
        if (creatingSongwriterFolderResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(creatingSongwriterFolderResult.Error.Message);
            return;
        }

        var songwriterService = scope.ServiceProvider.GetRequiredService<ISongwriterService>();
        var addingResult = await songwriterService.SaveFromFolderAsync(creatingSongwriterFolderResult.Value);
        if (addingResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(addingResult.Error.Message);
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Songwriters.Add(addingResult.Value.ToViewModel());
            ReplaceMovieReleasesDuplicates();
        });
        MessageBoxHelper.ShowInfoBox("Success");
    }

    [RelayCommand]
    private void AddSongwriter() => _dialogService.ShowDialog();

    #endregion

    protected override async void OnActivated()
    {
        base.OnActivated(); // register messages handlers

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

        ReplaceMovieReleasesDuplicates();
    }

    private void ReplaceMovieReleasesDuplicates()
    {
        var duplicates = Songwriters.SelectMany(e => e.Movies).SelectMany(e => e.MoviesReleases).GroupBy(e => e.DiscId).Where(e => e.Count() > 1);
        var movies = Songwriters.SelectMany(e => e.Movies);

        foreach (var movie in movies)
        {
            foreach (var moviesReleases in duplicates)
            {
                var mrToSwap = movie.MoviesReleases.FirstOrDefault(e => e.DiscId == moviesReleases.Key);
                if (mrToSwap is not null)
                {
                    int index = movie.MoviesReleases.IndexOf(mrToSwap);
                    movie.MoviesReleases[index] = moviesReleases.First();
                }
            }
        }
    }

    public async void Receive(SongwriterCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Songwriters.Add(message.SongwriterViewModel);
        });
    }
}

