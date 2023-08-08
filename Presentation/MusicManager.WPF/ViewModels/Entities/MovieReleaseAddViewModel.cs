using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities
{
    internal partial class MovieReleaseAddViewModel : DialogViewModel<MovieReleaseAddWindow>
    {
        private readonly IMovieReleaseService _movieReleaseService;
        private readonly IMovieService _movieService;

        [ObservableProperty]
        private ObservableCollection<MovieLookupDTO>? _movies;

        [ObservableProperty]
        private ObservableCollection<string>? _discTypes;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
        private MovieLookupDTO? _selectedMovie;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
        private string? _selectedDiscType;

        [ObservableProperty]
        private string _identifier = string.Empty;

        public MovieReleaseAddViewModel(
            IUserDialogService<MovieReleaseAddWindow> dialogService,
            IMovieReleaseService movieReleaseService,
            IMovieService movieService) : base(dialogService)
        {
            _movieReleaseService = movieReleaseService;
            _movieService = movieService;
        }

        protected override async Task Accept()
        {
            var dto = new MovieReleaseAddDTO(SelectedMovie!.MovieId, Identifier, SelectedDiscType!.CreateDiscType().Value);
            var addingResult = await _movieReleaseService.SaveAsync(dto);

            if (addingResult.IsSuccess)
            {
                var message = new MovieReleaseAddedMessage(new MovieReleaseViewModel
                {
                    DiscId = addingResult.Value,
                    MoviesLinks = new List<MovieId> { dto.MovieId },
                    DiscType = SelectedDiscType!,
                    Identificator = dto.Identifier
                });

                Messenger.Send(message);
            }
            else
            {
                MessageBoxHelper.ShowErrorBox(addingResult.Error.Message);
            }

            _dialogService.CloseDialog();
        }

        protected override bool CanAccept()
        {
            return NullValidator.IsAllNotNull(SelectedDiscType, SelectedMovie);
        }

        protected override async void OnActivated()
        {
            var moviesResult = await _movieService.GetLookupsAsync();
            if (moviesResult.IsSuccess)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Movies = new(moviesResult.Value);
                });
            }

            DiscTypes = new(Enum.GetValues<DiscType>().Select(e => e.MapToString()));
        }
    }
}
