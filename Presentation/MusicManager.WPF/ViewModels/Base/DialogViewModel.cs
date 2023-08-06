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

    protected DialogViewModel(IUserDialogService<TWindow> dialogService) : base()
    {
        _dialogService = dialogService;
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    protected abstract Task Accept();

    protected abstract bool CanAccept();

    [RelayCommand]
    protected virtual void Cancel() => _dialogService.CloseDialog();
}
