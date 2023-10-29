using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Infrastructure;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class MoviesPanelViewModel : 
    ObservableRecipient,
    IRecipient<MovieCreatedMessage>,
    IRecipient<ExistingMovieReleaseAddToMovieRequest>
{

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public IReadOnlyCollection<MovieViewModel> Movies => new ObservableCollection<MovieViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(s => s.Movies));

    private readonly IUserDialogService<MovieAddWindow> _movieAddDialogService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ObservableCollection<DirectorViewModel> EnableDirectors { get; } = new ObservableCollection<DirectorViewModel>();

    public MoviesPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public MoviesPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel,
        IUserDialogService<MovieAddWindow> dialogService,
        IServiceScopeFactory serviceScopeFactory) 
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        _movieAddDialogService = dialogService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMovieReleaseCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteMovieCommand))]
    private MovieViewModel? _selectedMovie;

    [RelayCommand]
    private void AddMovie() => _movieAddDialogService.ShowDialog();

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteMovie()
    {
        var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedMovie!.Title} from list?");
        if (dialog == MessageBoxResult.Yes)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMovieService>();
            var result = await service.DeleteAsync(SelectedMovie!.MovieId);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(result.Error.Message);
                return;
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                var songwriter = SongwritersPanelViewModel.Songwriters.FirstOrDefault(e => e.SongwriterId == SelectedMovie.SongwriterId);
                songwriter?.Movies.Remove(SelectedMovie);
            });
        }
    }

    private bool CanDelete() => SelectedMovie is not null;

    [RelayCommand(CanExecute = nameof(CanAddMovieRelease))]
    private async Task AddMovieRelease()
    {
        if (AddMovieReleaseCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var movieReleaseService = scope.ServiceProvider.GetRequiredService<IMovieReleaseService>();
        var lookupsResult = await movieReleaseService.GetLookupsAsync();
        if (lookupsResult.IsSuccess)
        {
			var dialogService = scope.ServiceProvider.GetRequiredService<IUserDialogService<MovieReleaseMovieWindow>>();
			var dataContext = scope.ServiceProvider.GetRequiredService<MovieReleaseAddToMovieViewModel>();

            var moviesReleaseToSelectFrom = lookupsResult.Value.Where(e => !SelectedMovie!.MoviesReleasesLinks.Select(e => e.MovieRelease).Any(it => it.DiscId == e.Id));
			dataContext.MoviesReleasesToSelectFrom = moviesReleaseToSelectFrom;
			dialogService.ShowDialog(dataContext);
		}
        else
        {
            MessageBoxHelper.ShowErrorBox(lookupsResult.Error.Message);
        }

		

        //var movieReleasesToSelectFrom = Movies
        //    .SelectMany(e => e.MoviesReleasesLinks.Select(e => e.MovieRelease))
        //    .DistinctBy(e => e.DiscId)
        //    .Where(e => !SelectedMovie!.MoviesReleasesLinks.Select(e => e.MovieRelease).Contains(e));
    }

	[RelayCommand]
    private async Task Save()
    {
        if (SaveCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
        var moviesToUpdate = Movies.Where(e => e.IsModified);

        var results = new List<Result>();
        foreach (var item in moviesToUpdate)
        {
            var dto = new MovieUpdateDTO(
                item.MovieId,
                item.Title,
                item.ProductionCountry,
                item.ProductionYear,
                item.Director?.Id);

            var updateResult = await movieService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                item.RollBackChanges();
            }
            else
            {
                item.SetCurrentAsPrevious();
            }

            results.Add(updateResult);
        }

        if (results.Any(e => e.IsFailure))
        {
            MessageBoxHelper.ShowErrorBox(string.Join(",", results.Where(e => e.IsFailure).Select(e => e.Error.Message)));
        }
        else
        {
            MessageBoxHelper.ShowInfoBox("Successfully updated.");
        }
    }

    [RelayCommand]
    private async Task AddDirector(string fullName)
    {
        if (AddDirectorCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDirectorService>();
        var result = await service.SaveAsync(fullName);
        if (result.IsSuccess)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                EnableDirectors.Add(new DirectorViewModel
                {
                    Id = result.Value,
                    FullName = fullName,
                });
            });
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(result.Error.Message);
        }
    }

    private bool CanAddMovieRelease()
    {
        return SelectedMovie is not null;
    }

	[RelayCommand]
	private async Task SelectionChanged(MovieViewModel movie)
	{
		if (movie is null)
		{
			return;
		}

		if (!movie.IsReleasesLinksLoaded)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var movieReleaseService = scope.ServiceProvider.GetRequiredService<IMovieReleaseService>();
			var moviesReleasesLinksResult = await movieReleaseService.GetLinksAsync(movie.MovieId);
			if (moviesReleasesLinksResult.IsSuccess)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					movie.MoviesReleasesLinks = new(moviesReleasesLinksResult.Value.Select(e => e.ToViewModel()));
					movie.IsReleasesLinksLoaded = true;

                    ReplaceMovieReleasesDuplicates();
				});
			}
		}
	}

	private void ReplaceMovieReleasesDuplicates()
	{
		var duplicates = Movies
			.SelectMany(e => e.MoviesReleasesLinks.Select(e => e.MovieRelease))
			.GroupBy(e => e.DiscId)
			.Where(e => e.Count() > 1);

		foreach (var movie in Movies)
		{
			foreach (var moviesReleases in duplicates)
			{
				var mrToSwap = movie.MoviesReleasesLinks.FirstOrDefault(e => e.MovieRelease.DiscId == moviesReleases.Key);
				if (mrToSwap is not null)
				{
					mrToSwap.MovieRelease = moviesReleases.First();
				}
			}
		}
	}

	public async void Receive(ExistingMovieReleaseAddToMovieRequest request)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
        var addingResult = await movieService.AddReleaseAsync(new MovieReleaseMovieDTO(SelectedMovie!.MovieId, request.SelectedMovieReleaseId));

		if (addingResult.IsSuccess)
		{
			await Application.Current.Dispatcher.InvokeAsync(async () =>
			{
				var viewModel = SongwritersPanelViewModel.Songwriters
				.SelectMany(e => e.Movies.SelectMany(e => e.MoviesReleasesLinks))
				.Select(e => e.MovieRelease)
				.FirstOrDefault(e => e.DiscId == request.SelectedMovieReleaseId);

				if (viewModel is null)
				{
                    var movieReleaseService = scope.ServiceProvider.GetRequiredService<IMovieReleaseService>();
                    var result = await movieReleaseService.GetAsync(request.SelectedMovieReleaseId);
                    if (result.IsSuccess)
                    {
						SelectedMovie.MoviesReleasesLinks.Add(new MovieReleaseLinkViewModel { MovieRelease = result.Value.ToViewModel(), IsFolder = false });
					}
				}
				else
				{
					SelectedMovie.MoviesReleasesLinks.Add(new MovieReleaseLinkViewModel { MovieRelease = viewModel, IsFolder = false });
				}
			});
		}
		else
        {
            MessageBoxHelper.ShowErrorBox(addingResult.Error.Message);
        }
    }

    public async void Receive(MovieCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var songwriter = SongwritersPanelViewModel
            .Songwriters
            .FirstOrDefault(e => e.SongwriterId == message.MovieViewModel.SongwriterId);

            if (songwriter is not null)
            {
                message.MovieViewModel.SetCurrentAsPrevious();
                songwriter.Movies.Add(message.MovieViewModel);
            }
        });
    }

    protected override async void OnActivated()
    {
        base.OnActivated();

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDirectorService>();
        var result = await service.GetAllAsync();

        if (result.IsSuccess)
        {
            EnableDirectors.AddRange(result.Value.Select(e => e.ToViewModel()));
        }
    }
}
