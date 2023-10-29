using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
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
    protected MovieReleaseLookupDTO? _selectedMovieRelease;

    private IEnumerable<MovieReleaseLookupDTO>? _movieReleasesToSelectFrom;
    public IEnumerable<MovieReleaseLookupDTO> MoviesReleasesToSelectFrom 
    {
        get => _movieReleasesToSelectFrom ??= Enumerable.Empty<MovieReleaseLookupDTO>();
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

    protected virtual bool CanAccept() => SelectedMovieRelease is not null;
}
