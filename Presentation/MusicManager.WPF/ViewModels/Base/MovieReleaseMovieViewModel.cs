﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Base;

internal abstract partial class MovieReleaseMovieViewModel : ObservableRecipient
{
    protected readonly IUserDialogService<MovieReleaseMovieWindow> _movieReleaseMovieWindowService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    protected MovieReleaseViewModel? _selectedViewModel;

    public ObservableCollection<MovieReleaseViewModel>? MoviesReleasesToSelectFrom { get; set; }

    public MovieReleaseMovieViewModel(
        IEnumerable<MovieReleaseViewModel> movieReleases,
		IUserDialogService<MovieReleaseMovieWindow> dialogService) : base()
    {
        MoviesReleasesToSelectFrom = new (movieReleases);
        _movieReleaseMovieWindowService = dialogService;
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    protected abstract void Accept();

    protected virtual bool CanAccept() => SelectedViewModel is not null;
}
