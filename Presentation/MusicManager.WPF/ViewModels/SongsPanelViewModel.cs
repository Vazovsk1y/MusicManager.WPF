using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;

namespace MusicManager.WPF.ViewModels;

internal class SongsPanelViewModel : ObservableObject
{
    public DiscsPanelViewModel DiscsPanelViewModel { get; }

    public IReadOnlyCollection<SongViewModel> Songs => new ObservableCollection<SongViewModel>(DiscsPanelViewModel.Compilations
        .SelectMany(e => e.Songs)
        .Union(DiscsPanelViewModel.MovieReleases.SelectMany(e => e.Songs)));

    private SongViewModel? _selectedSong;

    public SongsPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

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


public static class InvalidOperationExceptionHelper
{
    public static void ThrowIfTrue(bool condition, string exceptionText)
    {
        if (condition)
        {
            throw new InvalidOperationException(exceptionText);
        }
    }
}


