using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Infrastructure;
using MusicManager.WPF.Views.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieReleaseAddViewModel : DialogViewModel<MovieReleaseAddWindow>
{
	private readonly IMovieReleaseService _movieReleaseService;
	private readonly IMovieService _movieService;

	[ObservableProperty]
	private ObservableCollection<MovieLookupDTO>? _movies;

	[ObservableProperty]
	private ObservableCollection<DiscType>? _discTypes;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string? _selectedCountry;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private int? _selectedYear;

	private MovieLookupDTO? _selectedMovie;
	public MovieLookupDTO? SelectedMovie
	{
		get => _selectedMovie;
		set
		{
			if (SetProperty(ref _selectedMovie, value) &&
				value is not null
				&& !SelectedMovies.Contains(value))
			{
				AcceptCommand.NotifyCanExecuteChanged();
				SelectedMovies.Add(value);
			}
		}
	}

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private DiscType? _selectedDiscType;

	[ObservableProperty]
	private string _identifier = string.Empty;

	private readonly ObservableCollection<MovieLookupDTO> _selectedMovies = new();

	public ObservableCollection<MovieLookupDTO> SelectedMovies => _selectedMovies;

	public MovieReleaseAddViewModel(
		IUserDialogService<MovieReleaseAddWindow> dialogService,
		IMovieReleaseService movieReleaseService,
		IMovieService movieService,
		UserConfigViewModel settingsViewModel) : base(dialogService, settingsViewModel)
	{
		_movieReleaseService = movieReleaseService;
		_movieService = movieService;
	}

	protected override async Task Accept()
	{
		var moviesLinks = SelectedMovies.Select(e => e.MovieId).ToList();
		var dto = new MovieReleaseAddDTO(moviesLinks, Identifier, SelectedDiscType!, SelectedYear, SelectedCountry);
		var addingResult = await _movieReleaseService.SaveAsync(dto, _settingsViewModel.CreateAssociatedFolder);

		if (addingResult.IsSuccess)
		{
			var message = new MovieReleaseCreatedMessage(new MovieReleaseViewModel
			{
				DiscId = addingResult.Value,
				SelectedDiscType = SelectedDiscType!,
				Identifier = dto.Identifier,
				ProductionYear = SelectedYear,
				ProductionCountry = SelectedCountry
			}, moviesLinks);

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
				Movies = new(moviesResult.Value.OrderBy(e => e.Title));
			});
		}

		DiscTypes = new(DiscType.EnumerateRange());
	}
}
