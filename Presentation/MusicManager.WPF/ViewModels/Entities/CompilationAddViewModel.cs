using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class CompilationAddViewModel : DialogViewModel<CompilationAddWindow>
{
    private readonly ISongwriterService _songwriterService;
    private readonly ICompilationService _compilationService;

    [ObservableProperty]
    private ObservableCollection<SongwriterLookupDTO>? _songwriters;

    [ObservableProperty]
    private ObservableCollection<DiscType>? _discTypes;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private SongwriterLookupDTO? _selectedSongwriter;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private DiscType? _selectedDiscType;

    [ObservableProperty]
    private string _identifier = string.Empty;

    public CompilationAddViewModel(
        IUserDialogService<CompilationAddWindow> dialogService,
        ISongwriterService songwriterService,
        ICompilationService compilationService) : base(dialogService)
    {
        _songwriterService = songwriterService;
        _compilationService = compilationService;
    }

    protected override async Task Accept()
    {
        var dto = new CompilationAddDTO(SelectedSongwriter!.Id, Identifier, SelectedDiscType!);
        var addingResult = await _compilationService.SaveAsync(dto);
        if (addingResult.IsSuccess)
        {
            var message = new CompilationCreatedMessage(new CompilationViewModel
            {
                DiscId = addingResult.Value,
                SongwriterId = dto.SongwriterId,
                Identificator = dto.Identifier,
                DiscType = SelectedDiscType!.Value,
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
        return NullValidator.IsAllNotNull(SelectedSongwriter, SelectedDiscType);
    }

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

        DiscTypes = new(DiscType.EnumerateRange());
    }
}
