using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicManager.WPF.ViewModels;

internal class DiscsPanelViewModel : ObservableObject
{
    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public MoviesPanelViewModel MoviesPanelViewModel { get; }

    public IReadOnlyCollection<CompilationViewModel> Compilations => 
        new ObservableCollection<CompilationViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(e => e.Compilations));

    public IReadOnlyCollection<MovieReleaseViewModel> MovieReleases => 
        new ObservableCollection<MovieReleaseViewModel>(MoviesPanelViewModel.Movies.SelectMany(e => e.MoviesReleases));

    private DiscViewModel? _selectedDisc;

    public DiscsPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public DiscsPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel,
        MoviesPanelViewModel moviesPanelViewModel)
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        MoviesPanelViewModel = moviesPanelViewModel;
    }

    public DiscViewModel? SelectedDisc
    {
        get => _selectedDisc;
        set => SetProperty(ref _selectedDisc, value);
    }
}