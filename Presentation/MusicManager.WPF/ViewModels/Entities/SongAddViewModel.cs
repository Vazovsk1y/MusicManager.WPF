using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Domain.Constants;
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
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectPathCommand))]
    private FileInfo? _selectedSongPath;

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
        ISongFileFactory songFileFactory) : base(dialogService)
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
        if (IsFromCue)
        {
            var dtoResult = _songFileFactory.Create(null, SelectedSongPath);
            if (dtoResult.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(dtoResult.Error.Message);
                _dialogService.CloseDialog();
            }

            var cueResult = await _songService.SaveFromFileAsync(dtoResult.Value, SelectedDisc!.DiscId, false);
            if (cueResult.IsSuccess)
            {
                Messenger.Send(new SongCreatedMessage(SelectedDisc!.DiscId, cueResult.Value.Select(e => e.ToViewModel())));
            }
            else
            {
                MessageBoxHelper.ShowErrorBox(cueResult.Error.Message);
            }
        }
        else
        {
            var dtoResult = _songFileFactory.Create(SelectedSongPath);
            if (dtoResult.IsFailure)
            {
                MessageBoxHelper.ShowErrorBox(dtoResult.Error.Message);
                _dialogService.CloseDialog();
            }

            var result = await _songService.SaveFromFileAsync(dtoResult.Value, SelectedDisc!.DiscId, false);
            if (result.IsSuccess)
            {
                Messenger.Send(new SongCreatedMessage(SelectedDisc!.DiscId, result.Value.Select(e => e.ToViewModel())));
            }
            else
            {
                MessageBoxHelper.ShowErrorBox(result.Error.Message);
            }
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
    }
}
