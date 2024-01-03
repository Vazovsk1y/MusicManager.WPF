using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Infrastructure;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class DiscsPanelViewModel :
    ObservableRecipient,
    IRecipient<CompilationCreatedMessage>,
    IRecipient<MovieReleaseCreatedMessage>
{
    private const int DiscTypeMaxCount = 15;

    public SongwirtersPanelViewModel SongwritersPanelViewModel { get; }

    public MoviesPanelViewModel MoviesPanelViewModel { get; }

    public IReadOnlyCollection<CompilationViewModel> Compilations =>
        new ObservableCollection<CompilationViewModel>(SongwritersPanelViewModel.Songwriters.SelectMany(e => e.Compilations));

    public IReadOnlyCollection<MovieReleaseViewModel> MovieReleases =>
        new ObservableCollection<MovieReleaseViewModel>(MoviesPanelViewModel.Movies.SelectMany(e => e.MoviesReleasesLinks.Select(e => e.MovieRelease)));

    public IReadOnlyCollection<IDiscViewModel> Discs => new ObservableCollection<IDiscViewModel>(
        MovieReleases.Cast<IDiscViewModel>()
        .Union(Compilations.Cast<IDiscViewModel>()));

    private readonly IUserDialogService<CompilationAddWindow> _compilationDialogService;
    private readonly IUserDialogService<MovieReleaseAddWindow> _movieReleaseDialogService;
    private readonly IUserDialogService<SongAddWindow> _songAddDialogService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private IDiscViewModel? _selectedDisc;

    public IEnumerable<DiscType> EnableDiscTypes => DiscType.EnumerateRange(DiscTypeMaxCount);


    public DiscsPanelViewModel()
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public DiscsPanelViewModel(
        SongwirtersPanelViewModel songwritersPanelViewModel,
        MoviesPanelViewModel moviesPanelViewModel,
        IUserDialogService<CompilationAddWindow> dialogService,
        IUserDialogService<MovieReleaseAddWindow> movieReleaseDialogService,
        IUserDialogService<SongAddWindow> songAddDialogService,
        IServiceScopeFactory serviceScopeFactory) : base()
    {
        SongwritersPanelViewModel = songwritersPanelViewModel;
        MoviesPanelViewModel = moviesPanelViewModel;
        _compilationDialogService = dialogService;
        _movieReleaseDialogService = movieReleaseDialogService;
        _serviceScopeFactory = serviceScopeFactory;
        _songAddDialogService = songAddDialogService;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCompilationCommand))]
    private CompilationViewModel? _selectedCompilation;

    [ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteMovieReleaseCommand))]
	private MovieReleaseViewModel? _selectedMovieRelease;

    private object? _selectedItem;
    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value is MovieReleaseLinkViewModel movieReleaseLinkViewModel)
            {
                SelectedDisc = movieReleaseLinkViewModel.MovieRelease;
                _selectedItem = value;
            }
            else
            {
                _selectedItem = value;
            }
        }
    }

    public IDiscViewModel? SelectedDisc
    {
        get => _selectedDisc;
        set
        {
            if (SetProperty(ref _selectedDisc, value))
            {
                AddSongCommand.NotifyCanExecuteChanged();

				switch (value)
                {
                    case CompilationViewModel compilationViewModel:
                        {
                            SelectedCompilation = compilationViewModel;
                            SelectedMovieRelease = null;
                        }
                        break;
                    case MovieReleaseViewModel movieReleaseViewModel:
                        {
                            SelectedMovieRelease = movieReleaseViewModel;
                            SelectedCompilation = null;
                        }
                        break;
                    default:
                        {
                            SelectedMovieRelease = null;
                            SelectedCompilation = null;
                        }
                        break;
                }
            }
        }
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

    [RelayCommand(CanExecute = nameof(CanDeleteCompilation))]
    private async Task DeleteCompilation()
    {
        var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedCompilation!.Identifier} from list?");
        if (dialog == MessageBoxResult.Yes)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICompilationService>();
            var result = await service.DeleteAsync(SelectedCompilation!.DiscId);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(result.Error.Message);
                return;
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                var songwriter = SongwritersPanelViewModel.Songwriters.FirstOrDefault(e => e.SongwriterId == SelectedCompilation.SongwriterId);
                songwriter?.Compilations.Remove(SelectedCompilation);
            });
        }
    }

    private bool CanDeleteCompilation() => SelectedCompilation is not null;

	[RelayCommand(CanExecute = nameof(CanDeleteMovieRelease))]
	private async Task DeleteMovieRelease()
	{
		var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedMovieRelease!.Identifier} from list?");
		if (dialog == MessageBoxResult.Yes)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var service = scope.ServiceProvider.GetRequiredService<IMovieReleaseService>();
			var result = await service.DeleteAsync(SelectedMovieRelease!.DiscId);
			if (result.IsFailure)
			{
				MessageBoxHelper.ShowErrorBox(result.Error.Message);
				return;
			}

			await App.Current.Dispatcher.InvokeAsync(() =>
			{
                var movies = MoviesPanelViewModel.Movies.Where(e => e.MoviesReleasesLinks.Any(e => e.MovieRelease.DiscId == SelectedMovieRelease.DiscId));
                foreach (var movie in movies)
                {
                    movie.MoviesReleasesLinks.Remove(movie.MoviesReleasesLinks.First(e => e.MovieRelease.DiscId == SelectedMovieRelease.DiscId));
                }
			});
		}
	}

	private bool CanDeleteMovieRelease() => SelectedMovieRelease is not null;

	[RelayCommand]
    private async Task SaveCompilations()
    {
		var compilationsToUpdate = Compilations.Where(e => e.IsModified).ToList();
		if (SaveCompilationsCommand.IsRunning || compilationsToUpdate.Count == 0)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var compilationService = scope.ServiceProvider.GetRequiredService<ICompilationService>();
        var results = new List<Result>();
        foreach (var compilation in compilationsToUpdate)
        {
            var dto = new CompilationUpdateDTO
            (
                compilation.DiscId,
                compilation.Identifier,
                compilation.ProductionCountry,
                compilation.ProductionYear,
                compilation.SelectedDiscType
            );

            var updateResult = await compilationService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                compilation.RollBackChanges();
            }
            else
            {
                compilation.SaveState();
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
    private async Task SaveMoviesReleases()
    {
		var moviesReleasesToUpdate = MovieReleases.Where(e => e.IsModified).ToList();
		if (SaveMoviesReleasesCommand.IsRunning || moviesReleasesToUpdate.Count == 0)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var movieReleaseService = scope.ServiceProvider.GetRequiredService<IMovieReleaseService>();

        var results = new List<Result>();
        foreach (var release in moviesReleasesToUpdate)
        {
            var dto = new MovieReleaseUpdateDTO
            (
                release.DiscId,
                release.Identifier,
                release.ProductionCountry,
                release.ProductionYear,
                release.SelectedDiscType
            );

            var updateResult = await movieReleaseService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                release.RollBackChanges();
            }
            else
            {
                release.SaveState();
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


    [RelayCommand(CanExecute = nameof(CanAddSong))]
    private void AddSong()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<SongAddViewModel>();
        dataContext.SelectedDisc = SelectedDisc!;
        _songAddDialogService.ShowDialog(dataContext);
    }

    private bool CanAddSong() => SelectedDisc is not null;

    public async void Receive(CompilationCreatedMessage message)
    {
        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            var songwriter = SongwritersPanelViewModel.Songwriters.FirstOrDefault(e => e.SongwriterId == message.CompilationViewModel.SongwriterId);

            if (songwriter is not null)
            {
                songwriter.Compilations.Add(message.CompilationViewModel);
                message.CompilationViewModel.SaveState();
                songwriter.Compilations = new(songwriter.Compilations.OrderBy(e => e.ProductionYear));
            }
        });
    }

	public async void Receive(MovieReleaseCreatedMessage message)
	{
		await App.Current.Dispatcher.InvokeAsync(() =>
		{
			foreach (var movieLink in message.MoviesLinks)
			{
				var movie = MoviesPanelViewModel.Movies.FirstOrDefault(e => movieLink.MovieId == e.MovieId);
				movie!.MoviesReleasesLinks.Add(new MovieReleaseLinkViewModel { MovieRelease = message.MovieReleaseViewModel, IsFolder = movieLink.AddReleaseAsFolder });
				message.MovieReleaseViewModel.SaveState();
                movie.MoviesReleasesLinks = new(movie.MoviesReleasesLinks.OrderBy(e => e.MovieRelease.ProductionYear));
			}
		});
	}

	[RelayCommand]
	private async Task MovieReleaseLinkSelectionChanged(MovieReleaseLinkViewModel movieReleaseLink)
	{
		if (movieReleaseLink is null)
		{
			return;
		}

		if (!movieReleaseLink.MovieRelease.IsSongsLoaded)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var songService = scope.ServiceProvider.GetRequiredService<ISongService>();
			var songsResult = await songService.GetAllAsync(movieReleaseLink.MovieRelease.DiscId);
			if (songsResult.IsSuccess)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					movieReleaseLink.MovieRelease.Songs.AddRange(songsResult.Value.Select(e => e.ToViewModel()));
					movieReleaseLink.MovieRelease.IsSongsLoaded = true;
				});
			}
		}
	}

	[RelayCommand]
	private async Task CompilationSelectionChanged(CompilationViewModel compilation)
	{
		if (compilation is null)
		{
			return;
		}

		if (!compilation.IsSongsLoaded)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var songService = scope.ServiceProvider.GetRequiredService<ISongService>();
			var songsResult = await songService.GetAllAsync(compilation.DiscId);
			if (songsResult.IsSuccess)
			{
				await Application.Current.Dispatcher.InvokeAsync(() =>
				{
					compilation.Songs.AddRange(songsResult.Value.Select(e => e.ToViewModel()));
					compilation.IsSongsLoaded = true;
				});
			}
		}
	}
}