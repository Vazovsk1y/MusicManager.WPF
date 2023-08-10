using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.Views.Windows;
using System.Threading.Tasks;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class SongwriterAddViewModel : DialogViewModel<SongwriterAddWindow>
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    private readonly ISongwriterService _songwriterService;

    public SongwriterAddViewModel(
        IUserDialogService<SongwriterAddWindow> dialogService, 
        ISongwriterService songwriterService) : base(dialogService)
    {
        _songwriterService = songwriterService;
    }

    protected override async Task Accept()
    {
        var dto = new SongwriterAddDTO(Name, LastName);
        var savingResult = await _songwriterService.SaveAsync(dto);

        if (savingResult.IsSuccess)
        {
            var message = new SongwriterCreatedMessage(
                new SongwriterViewModel
                {
                    SongwriterId = savingResult.Value,
                    FullName = $"{dto.Name} {dto.LastName}"
                });

            Messenger.Send(message);
        }
        else
        {
            MessageBoxHelper.ShowErrorBox(savingResult.Error.Message);
        }

        _dialogService.CloseDialog();
    }

    protected override bool CanAccept() => true;
}
