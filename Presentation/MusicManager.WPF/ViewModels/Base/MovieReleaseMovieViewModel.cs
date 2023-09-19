using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicManager.WPF.ViewModels.Base;

internal abstract partial class MovieReleaseMovieViewModel : ObservableRecipient
{
    protected readonly IUserDialogService<MovieReleaseMovieWindow> _movieReleaseMovieWindowService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    protected MovieReleaseViewModel? _selectedViewModel;

    private IEnumerable<MovieReleaseViewModel>? _movieReleasesToSelectFrom;
    public IEnumerable<MovieReleaseViewModel> MoviesReleasesToSelectFrom 
    {
        get => _movieReleasesToSelectFrom ??= Enumerable.Empty<MovieReleaseViewModel>();
        set
        {
            _movieReleasesToSelectFrom = value.OrderBy(e => e.Identifier);
        }
    }

    public MovieReleaseMovieViewModel(
		IUserDialogService<MovieReleaseMovieWindow> dialogService) : base()
    {
        _movieReleaseMovieWindowService = dialogService;
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    protected abstract void Accept();

    protected virtual bool CanAccept() => SelectedViewModel is not null;
}
