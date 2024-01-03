using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Utils;
using MusicManager.WPF.Infrastructure;
using MusicManager.WPF.Messages;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    private readonly IFileSystemManager _fileManagerInteractor;
    private readonly ISongwriterFolderFactory _songwriterFolderFactory;
    private readonly IUserDialogService<SongwriterAddWindow> _dialogService;
    private readonly UserConfigViewModel _settingsViewModel;

    public SongwirtersPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public SongwirtersPanelViewModel(
        IServiceScopeFactory serviceScopeFactory,
        IFileSystemManager fileManagerInteractor,
        ISongwriterFolderFactory songwriterFolderFactory,
        IUserDialogService<SongwriterAddWindow> dialogService,
        UserConfigViewModel settingsViewModel) : base()
    {

        _serviceScopeFactory = serviceScopeFactory;
        _fileManagerInteractor = fileManagerInteractor;
        _songwriterFolderFactory = songwriterFolderFactory;
        _dialogService = dialogService;
        _settingsViewModel = settingsViewModel;
    }



	[ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteSongwriterCommand))]
    private SongwriterViewModel? _selectedSongwriter;

    public ObservableCollection<SongwriterViewModel> Songwriters => _songwriters;

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
        });

        if (folders.Count > 0 && dtos.Count > 0) 
        {
            MessageBoxHelper.ShowInfoBox("Success");
        }

        AddSongwriterFromFolderCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddSongwriter() => _dialogService.ShowDialog();

    [RelayCommand(CanExecute = nameof(CanDeleteSongwriter))]
    private async Task DeleteSongwriter()
    {
        var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedSongwriter!.FullName} from list?");
        if (dialog == MessageBoxResult.Yes)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ISongwriterService>();
            var result = await service.DeleteAsync(SelectedSongwriter.SongwriterId);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(result.Error.Message);
                return;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Songwriters.Remove(SelectedSongwriter);
            });
        }
    }

    private bool CanDeleteSongwriter() => SelectedSongwriter is not null;

    #endregion

	[RelayCommand]
	private async Task SelectionChanged(SongwriterViewModel songwriter)
	{
        if (songwriter is null)
        {
            return;
        }

		using var scope = _serviceScopeFactory.CreateScope();
		if (!songwriter.IsMoviesLoaded)
        {
			var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
			var moviesResult = await movieService.GetAllAsync(songwriter.SongwriterId);
			if (moviesResult.IsSuccess)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					songwriter.Movies = new(moviesResult.Value.Select(e => e.ToViewModel()));
					songwriter.IsMoviesLoaded = true;
                });
			}
		}

        if (!songwriter.IsCompilationsLoaded)
        {
			var compilationService = scope.ServiceProvider.GetRequiredService<ICompilationService>();
			var compilationsResult = await compilationService.GetAllAsync(songwriter.SongwriterId);
			if (compilationsResult.IsSuccess)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					songwriter.Compilations = new(compilationsResult.Value.Select(e => e.ToViewModel()));
					songwriter.IsCompilationsLoaded = true;
				});
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

	protected override async void OnActivated()
	{
		base.OnActivated(); // register messages handlers

		using var scope = _serviceScopeFactory.CreateScope();
		var songwriterService = scope.ServiceProvider.GetRequiredService<ISongwriterService>();

		var result = await songwriterService.GetAllAsync();
		if (result.IsSuccess)
		{
			Songwriters.AddRange(result.Value.Select(e => e.ToViewModel()));
		}
	}
}

