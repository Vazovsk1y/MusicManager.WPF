using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicManager.WPF.ViewModels;

internal class MoviesPanelViewModel : ObservableObject
{
    private MovieViewModel? _selectedMovie;

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public IReadOnlyCollection<MovieViewModel> Movies => new ObservableCollection<MovieViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(s => s.Movies));

    public MoviesPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public MoviesPanelViewModel(SongwirtersPanelViewModel songwritersPanelViewModel)
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
    }

    public MovieViewModel? SelectedMovie
    {
        get => _selectedMovie;
        set => SetProperty(ref _selectedMovie, value);
    }
}
