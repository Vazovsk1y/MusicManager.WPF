using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicManager.Services;
using System.Threading.Tasks;
using System.Windows;

namespace MusicManager.WPF.ViewModels.Entities;

internal abstract partial class DialogViewModel<TWindow> : ObservableRecipient
    where TWindow : Window
{
    protected readonly IUserDialogService<TWindow> _dialogService;
    protected readonly UserConfigViewModel _settingsViewModel;

    protected DialogViewModel(
        IUserDialogService<TWindow> dialogService,
        UserConfigViewModel settingsViewModel) : base()
    {
        _dialogService = dialogService;
        _settingsViewModel = settingsViewModel;
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    protected abstract Task Accept();

    protected abstract bool CanAccept();

    [RelayCommand]
    protected virtual void Cancel() => _dialogService.CloseDialog();
}
