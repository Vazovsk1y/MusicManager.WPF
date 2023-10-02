using CommunityToolkit.Mvvm.Messaging;
using MusicManager.WPF.Messages;
using MusicManager.WPF.ViewModels.Base;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using MusicManager.Services;

namespace MusicManager.WPF.ViewModels;

internal class MovieReleaseAddToMovieViewModel : MovieReleaseMovieViewModel
{
    public MovieReleaseAddToMovieViewModel(
        IUserDialogService<MovieReleaseMovieWindow> dialogService) : base(dialogService)
    {
    }

    protected override void Accept()
    {
        var message = new ExistingMovieReleaseAddToMovieRequest(SelectedViewModel!);
        Messenger.Send(message);
        _movieReleaseMovieWindowService.CloseDialog();
    }
}
