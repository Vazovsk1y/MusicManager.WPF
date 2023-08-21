using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.Tools;

public interface IWpfWindowService<TWindow> where TWindow : Window
{
    void StartDialog<TViewModel>(TViewModel dataContext) where TViewModel : ObservableObject;

    void CloseDialog();
}
