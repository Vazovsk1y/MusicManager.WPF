using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Domain.Constants;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.Views.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class SongAddViewModel : DialogViewModel<SongAddWindow>
{
    private readonly IBaseDiscService _baseDiscService;
    private readonly ISongService _songService;
    private readonly IFileManagerInteractor _fileManagerInteractor;
    private readonly ISongFileFactory _songFileFactory;

    [ObservableProperty]
    private ObservableCollection<DiscLookupDTO>? _discs;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private DiscLookupDTO? _selectedDisc;

    [ObservableProperty]
    private DiscNumber? _selectedDiscNumber;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectPathCommand))]
    private FileInfo? _selectedSongPath;

    [ObservableProperty]
    private ObservableCollection<DiscNumber> _discNumbers;

    private bool _isFromCue;

    public bool IsFromCue
    {
        get => _isFromCue;
        set 
        {
            if (SetProperty(ref _isFromCue, value))
            {
                SelectPathCommand.Execute(null);
            }
        }
    }

    public SongAddViewModel(
        IUserDialogService<SongAddWindow> dialogService,
        IBaseDiscService baseDiscService,
        ISongService songService,
        IFileManagerInteractor fileManagerInteractor,
        ISongFileFactory songFileFactory,
        SettingsViewModel settingsViewModel) : base(dialogService, settingsViewModel)
    {
        _baseDiscService = baseDiscService;
        _songService = songService;
        _fileManagerInteractor = fileManagerInteractor;
        _songFileFactory = songFileFactory;
    }

    [RelayCommand(CanExecute = nameof(CanSelectPath))]
    private void SelectPath()
    {
        string filter = IsFromCue ?
            $"Cue files (*{DomainConstants.CueExtension}) | *{DomainConstants.CueExtension}"
            :
            $"Audio files " +
            $"(*{DomainConstants.WVExtension};*{DomainConstants.Mp3Extension};*{DomainConstants.ApeExtension};*{DomainConstants.FlacExtension}) | " +
            $"*{DomainConstants.WVExtension};*{DomainConstants.Mp3Extension};*{DomainConstants.ApeExtension};*{DomainConstants.FlacExtension}";

        var dialogResult = _fileManagerInteractor.SelectFile(filter);
        if (dialogResult.IsFailure)
        {
            return;
        }

        SelectedSongPath = dialogResult.Value;
    }

    private bool CanSelectPath() => SelectedSongPath is null;

    protected override async Task Accept()
    {
        var songFileResult = _songFileFactory.Create(SelectedSongPath!);
        if (songFileResult.IsFailure)
        {
            MessageBoxHelper.ShowErrorBox(songFileResult.Error.Message);
            _dialogService.CloseDialog();
        }

        var dto = new SongAddDTO(SelectedDisc!.DiscId, songFileResult.Value, SelectedDiscNumber);
        var saveResult = await _songService.SaveAsync(dto, _settingsViewModel.CreateAssociatedFolder); // move song file/s or not.
        if (saveResult.IsSuccess)
        {
            Messenger.Send(new SongCreatedMessage(SelectedDisc!.DiscId, saveResult.Value.Select(e => e.ToViewModel())));
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(saveResult.Error.Message);
        }

        _dialogService.CloseDialog();
    }

    protected override bool CanAccept()
    {
        return NullValidator.IsAllNotNull(SelectedDisc, SelectedSongPath);
    }

    protected override async void OnActivated()
    {
        var result = await _baseDiscService.GetLookupsAsync();
        if (result.IsSuccess)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Discs = new(result.Value);
            });
        }

        DiscNumbers = new (DiscNumber.EnumerateRange());
    }
}
