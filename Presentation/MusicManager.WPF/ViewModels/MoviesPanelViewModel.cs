using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
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
    IRecipient<MovieReleaseCreatedMessage>,
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

    private bool CanAddMovieRelease()
    {
        return SelectedMovie is not null;
    }

    public async void Receive(MovieReleaseCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var movies = Movies.Where(e => message.MovieReleaseViewModel.MoviesLinks.Contains(e.MovieId));

            foreach (var movie in movies)
            {
                movie.MoviesReleases.Add(message.MovieReleaseViewModel);
            }
        });
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
}



public interface IWpfWindowService<TWindow> where TWindow : Window
{
    void StartDialog<TViewModel>(TViewModel dataContext) where TViewModel : ObservableObject;

    void CloseDialog();
}


public class WpfWindowService<TWindow> : IWpfWindowService<TWindow> where TWindow : Window
{
    private TWindow? _window;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WpfWindowService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void StartDialog<TViewModel>(TViewModel dataContext) where TViewModel : ObservableObject
    {
        CloseDialog();

        var scope = _serviceScopeFactory.CreateScope();
        _window = scope.ServiceProvider.GetRequiredService<TWindow>();
        _window.DataContext = dataContext;
        _window.Closed += (_, _) => scope.Dispose();
        _window.ShowDialog();
    }

    public void CloseDialog()
    {
        _window?.Close();
        _window = null;
    }
}





