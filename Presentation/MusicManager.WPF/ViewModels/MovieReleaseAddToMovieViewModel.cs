using CommunityToolkit.Mvvm.Messaging;
using MusicManager.WPF.Messages;
using MusicManager.WPF.ViewModels.Base;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;

namespace MusicManager.WPF.ViewModels;

internal class MovieReleaseAddToMovieViewModel : MovieReleaseMovieViewModel
{
    public MovieReleaseAddToMovieViewModel(
        IEnumerable<MovieReleaseViewModel> movieReleases, 
        IWpfWindowService<MovieReleaseMovieWindow> wpfWindowService) : base(movieReleases, wpfWindowService)
    {
    }

    protected override void Accept()
    {
        var message = new ExistingMovieReleaseAddToMovieRequest(SelectedViewModel!);
        Messenger.Send(message);
        _movieReleaseMovieWindowService.CloseDialog();
    }
}
