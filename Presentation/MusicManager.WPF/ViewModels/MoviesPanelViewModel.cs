using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Infrastructure;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class MoviesPanelViewModel : 
    ObservableRecipient,
    IRecipient<MovieCreatedMessage>,
    IRecipient<ExistingMovieReleaseAddToMovieRequest>
{

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public IReadOnlyCollection<MovieViewModel> Movies => new ObservableCollection<MovieViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(s => s.Movies));

    private readonly IUserDialogService<MovieAddWindow> _movieAddDialogService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ObservableCollection<DirectorViewModel> EnableDirectors { get; } = new ObservableCollection<DirectorViewModel>();

    public MoviesPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public MoviesPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel,
        IUserDialogService<MovieAddWindow> dialogService,
        IServiceScopeFactory serviceScopeFactory)
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        _movieAddDialogService = dialogService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMovieReleaseCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteMovieCommand))]
    private MovieViewModel? _selectedMovie;

    [RelayCommand]
    private void AddMovie() => _movieAddDialogService.ShowDialog();

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteMovie()
    {
        var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedMovie!.Title} from list?");
        if (dialog == MessageBoxResult.Yes)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMovieService>();
            var result = await service.DeleteAsync(SelectedMovie!.SongwriterId, SelectedMovie!.MovieId);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(result.Error.Message);
                return;
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                var songwriter = SongwritersPanelViewModel.Songwriters.FirstOrDefault(e => e.SongwriterId == SelectedMovie.SongwriterId);
                songwriter?.Movies.Remove(SelectedMovie);
            });
        }
    }

    private bool CanDelete() => SelectedMovie is not null;

    [RelayCommand(CanExecute = nameof(CanAddMovieRelease))]
    private void AddMovieRelease()
    {
        var movieReleasesToSelectFrom = Movies
            .SelectMany(e => e.MoviesReleases)
            .DistinctBy(e => e.DiscId)
            .Where(e => !SelectedMovie!.MoviesReleases.Contains(e));

        using var scope = _serviceScopeFactory.CreateScope();
        var dialogService = scope.ServiceProvider.GetRequiredService<IUserDialogService<MovieReleaseMovieWindow>>();
        var dataContext = scope.ServiceProvider.GetRequiredService<MovieReleaseAddToMovieViewModel>();
        dataContext.MoviesReleasesToSelectFrom = new (movieReleasesToSelectFrom);
        dialogService.ShowDialog(dataContext);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SaveCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
        var moviesToUpdate = Movies.Where(e => e.IsUpdatable);

        var results = new List<Result>();
        foreach (var item in moviesToUpdate)
        {
            var dto = new MovieUpdateDTO(
                item.MovieId,
                item.Title,
                item.ProductionCountry,
                item.ProductionYear,
                item.Director?.Id);

            var updateResult = await movieService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                item.RollBackChanges();
            }
            else
            {
                item.SetCurrentAsPrevious();
            }

            results.Add(updateResult);
        }

        if (results.Any(e => e.IsFailure))
        {
            MessageBoxHelper.ShowErrorBox(string.Join(",", results.Where(e => e.IsFailure).Select(e => e.Error.Message)));
        }
        else
        {
            MessageBoxHelper.ShowInfoBox("Successfully updated.");
        }
    }

    [RelayCommand]
    private async Task AddDirector(string fullName)
    {
        if (AddDirectorCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDirectorService>();
        var result = await service.SaveAsync(fullName);
        if (result.IsSuccess)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                EnableDirectors.Add(new DirectorViewModel
                {
                    Id = result.Value,
                    FullName = fullName,
                });
            });
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(result.Error.Message);
        }
    }

    private bool CanAddMovieRelease()
    {
        return SelectedMovie is not null;
    }

    public async void Receive(ExistingMovieReleaseAddToMovieRequest request)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
        var addingResult = await movieService.AddExistingMovieRelease(new ExistingMovieReleaseToMovieDTO(SelectedMovie!.MovieId, request.MovieReleaseViewModel.DiscId));

        if (addingResult.IsSuccess)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //request.MoviesLinks.Add(SelectedMovie.MovieId);
                SelectedMovie.MoviesReleases.Add(request.MovieReleaseViewModel);
            });
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(addingResult.Error.Message);
        }
    }

    public async void Receive(MovieCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var songwriter = SongwritersPanelViewModel
            .Songwriters
            .FirstOrDefault(e => e.SongwriterId == message.MovieViewModel.SongwriterId);

            if (songwriter is not null)
            {
                message.MovieViewModel.SetCurrentAsPrevious();
                songwriter.Movies.Add(message.MovieViewModel);
            }
        });
    }
    protected override async void OnActivated()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDirectorService>();
        var result = await service.GetAllAsync();

        if (result.IsSuccess)
        {
            EnableDirectors.AddRange(result.Value.Select(e => e.ToViewModel()));
        }
    }
}
