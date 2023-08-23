using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
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
    private MovieViewModel? _selectedMovie;

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public IReadOnlyCollection<MovieViewModel> Movies => new ObservableCollection<MovieViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(s => s.Movies));

    private readonly IUserDialogService<MovieAddWindow> _movieAddDialogService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

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

    public MovieViewModel? SelectedMovie
    {
        get => _selectedMovie;
        set
        {
            if (SetProperty(ref _selectedMovie, value))
            {
                AddMovieReleaseCommand.NotifyCanExecuteChanged();
            }
        }
    }

    [RelayCommand]
    private void AddMovie() => _movieAddDialogService.ShowDialog();

    [RelayCommand(CanExecute = nameof(CanAddMovieRelease))]
    private void AddMovieRelease()
    {
        var movieReleasesToSelectFrom = Movies
            .SelectMany(e => e.MoviesReleases)
            .DistinctBy(e => e.DiscId)
            .Where(e => !e.MoviesLinks.Contains(SelectedMovie!.MovieId));

        using var scope = _serviceScopeFactory.CreateScope();
        var dialogService = scope.ServiceProvider.GetRequiredService<IWpfWindowService<MovieReleaseMovieWindow>>();
        var dataContext = scope.ServiceProvider.GetRequiredService<MovieReleaseAddToMovieViewModel>();
        dataContext.MoviesReleasesToSelectFrom = new (movieReleasesToSelectFrom);
        dialogService.StartDialog(dataContext);
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
        var moviesToUpdate = Movies.Where(e => e.IsModified);

        var results = new List<Result>();
        foreach (var item in moviesToUpdate)
        {
            var dto = new MovieUpdateDTO(
                item.MovieId, 
                item.Title, 
                item.ProductionCountry, 
                item.ProductionYear, 
                item.DirectorName, 
                item.DirectorLastName);

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
                request.MovieReleaseViewModel.MoviesLinks.Add(SelectedMovie.MovieId);
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
}
