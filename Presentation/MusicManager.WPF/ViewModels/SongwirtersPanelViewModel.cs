using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels;

internal class SongwirtersPanelViewModel : ObservableRecipient
{
    private readonly ObservableCollection<SongwriterViewModel> _songwriters = new();

    private SongwriterViewModel? _selectedSongwriter;

    public ObservableCollection<SongwriterViewModel> Songwriters => _songwriters;

    public SongwriterViewModel? SelectedSongwriter
    {
        get => _selectedSongwriter;
        set => SetProperty(ref _selectedSongwriter, value);
    }
}

