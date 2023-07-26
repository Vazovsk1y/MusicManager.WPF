using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicManager.WPF.ViewModels;

internal class SongsPanelViewModel : ObservableObject
{
    public DiscsPanelViewModel DiscsPanelViewModel { get; }

    public IReadOnlyCollection<SongViewModel> Songs => new ObservableCollection<SongViewModel>(DiscsPanelViewModel.Compilations
        .SelectMany(e => e.Songs)
        .Union(DiscsPanelViewModel.MovieReleases.SelectMany(e => e.Songs)));

    private SongViewModel? _selectedSong;

    public SongsPanelViewModel(
        DiscsPanelViewModel discsPanelViewModel)
    {
        DiscsPanelViewModel = discsPanelViewModel;
    }

    public SongViewModel? SelectedSong
    {
        get => _selectedSong;
        set => SetProperty(ref _selectedSong, value);
    }
}
