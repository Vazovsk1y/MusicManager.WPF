using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Services;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class MoviesPanelViewModel : 
    ObservableRecipient,
    IRecipient<MovieReleaseAddedMessage>
{
    private MovieViewModel? _selectedMovie;

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public IReadOnlyCollection<MovieViewModel> Movies => new ObservableCollection<MovieViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(s => s.Movies));

    private readonly IUserDialogService<MovieAddWindow> _dialogService;

    public MoviesPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public MoviesPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel, 
        IUserDialogService<MovieAddWindow> dialogService)
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        _dialogService = dialogService;
    }

    public MovieViewModel? SelectedMovie
    {
        get => _selectedMovie;
        set => SetProperty(ref _selectedMovie, value);
    }

    [RelayCommand]
    private void AddMovie() => _dialogService.ShowDialog();

    public async void Receive(MovieReleaseAddedMessage message)
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
}
