using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.Views.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieAddViewModel : DialogViewModel<MovieAddWindow>
{
    #region --Fields--

    private readonly ISongwriterService _songwriterService;
    private readonly IMovieService _movieService;

    #endregion

    #region --Properties--

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private SongwriterLookupDTO? _selectedSongwriter;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private string? _selectedCountry;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private int? _selectedYear;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    public ObservableCollection<SongwriterLookupDTO>? _songwriters;

    #endregion

    #region --Constructors--

    public MovieAddViewModel(
       ISongwriterService songwriterService,
       IMovieService movieService,
       IUserDialogService<MovieAddWindow> dialogService,
       UserConfigViewModel settingsViewModel) : base(dialogService, settingsViewModel)
    {
        _songwriterService = songwriterService;
        _movieService = movieService;
    }

    #endregion

    #region --Commands--

    protected override async Task Accept()
    {
        var dto = new MovieAddDTO(SelectedSongwriter!.Id, (int)SelectedYear!, SelectedCountry!, Title);
        var saveResult = await _movieService.SaveAsync(dto, _settingsViewModel.CreateAssociatedFolder);

        if (saveResult.IsSuccess)
        {
            var vm = new MovieViewModel
            {
                Title = dto.Title,
                MovieId = saveResult.Value,
                SongwriterId = dto.SongwriterId,
                ProductionCountry = dto.ProductionCountry,
                ProductionYear = dto.ProductionYear
            };

            var message = new MovieCreatedMessage(vm);
            Messenger.Send(message);
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(saveResult.Error.Message);
        }

        _dialogService.CloseDialog();
    }

    protected override bool CanAccept() => NullValidator.IsAllNotNull(SelectedCountry, SelectedSongwriter, SelectedYear);

    #endregion

    #region --Methods--

    protected override async void OnActivated()
    {
        var songwritersResult = await _songwriterService.GetLookupsAsync();
        if (songwritersResult.IsSuccess)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Songwriters = new(songwritersResult.Value);
            });
        }
    }

    #endregion
}




