﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using MusicManager.Utils;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicManager.WPF.ViewModels;

internal partial class DiscsPanelViewModel : ObservableObject
{
    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public MoviesPanelViewModel MoviesPanelViewModel { get; }

    public IReadOnlyCollection<CompilationViewModel> Compilations => 
        new ObservableCollection<CompilationViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(e => e.Compilations));

    public IReadOnlyCollection<MovieReleaseViewModel> MovieReleases => 
        new ObservableCollection<MovieReleaseViewModel>(MoviesPanelViewModel.Movies.SelectMany(e => e.MoviesReleases));

    private readonly IUserDialogService<CompilationAddWindow> _compilationDialogService;
    private readonly IUserDialogService<MovieReleaseAddWindow> _movieReleaseDialogService;

    private DiscViewModel? _selectedDisc;

    public DiscsPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public DiscsPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel,
        MoviesPanelViewModel moviesPanelViewModel,
        IUserDialogService<CompilationAddWindow> dialogService,
        IUserDialogService<MovieReleaseAddWindow> movieReleaseDialogService)
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        MoviesPanelViewModel = moviesPanelViewModel;
        _compilationDialogService = dialogService;
        _movieReleaseDialogService = movieReleaseDialogService;
    }

    public DiscViewModel? SelectedDisc
    {
        get => _selectedDisc;
        set => SetProperty(ref _selectedDisc, value);
    }

    [RelayCommand]
    private void AddCompilation()
    {
        _compilationDialogService.ShowDialog();
    }

    [RelayCommand]
    private void AddMovieRelease()
    {
        _movieReleaseDialogService.ShowDialog();
    }
}