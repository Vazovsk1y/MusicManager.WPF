using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

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
    private readonly SettingsViewModel _settingsViewModel;

    public SongwirtersPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public SongwirtersPanelViewModel(
        IServiceScopeFactory serviceScopeFactory,
        IFileManagerInteractor fileManagerInteractor,
        ISongwriterFolderFactory songwriterFolderFactory,
        IUserDialogService<SongwriterAddWindow> dialogService,
        SettingsViewModel settingsViewModel) : base()
    {

        _serviceScopeFactory = serviceScopeFactory;
        _fileManagerInteractor = fileManagerInteractor;
        _songwriterFolderFactory = songwriterFolderFactory;
        _dialogService = dialogService;
        _settingsViewModel = settingsViewModel;
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
        new AsyncRelayCommand(OnSongwriterAddFromFolderExecute, () => !AddSongwriterFromFolderCommand.IsRunning && Songwriters.Count == 0);

    private async Task OnSongwriterAddFromFolderExecute()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var folders = new List<SongwriterFolder>();
        foreach (var item in new DirectoryInfo(_settingsViewModel.RootPath).EnumerateDirectories())
        {
            var creatingSongwriterFolderResult = _songwriterFolderFactory.Create(item);
            if (creatingSongwriterFolderResult.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(creatingSongwriterFolderResult.Error.Message);
                break;
            }
            else
            {
                folders.Add(creatingSongwriterFolderResult.Value);
            }
        }

        var dtos = new List<SongwriterDTO>();
        foreach (var item in folders)
        {
            var songwriterService = scope.ServiceProvider.GetRequiredService<ISongwriterService>();
            var addingResult = await songwriterService.SaveFromFolderAsync(item);
            if (addingResult.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(addingResult.Error.Message);
                break;
            }
            else
            {
                dtos.Add(addingResult.Value);   
            }
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Songwriters.AddRange(dtos.Select(e => e.ToViewModel()));
            ReplaceMovieReleasesDuplicates();
        });

        if (folders.Count > 0 && dtos.Count > 0) 
        {
            MessageBoxHelper.ShowInfoBox("Success");
        }
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

