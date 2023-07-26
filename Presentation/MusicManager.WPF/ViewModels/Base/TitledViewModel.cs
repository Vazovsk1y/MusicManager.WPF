using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.ViewModels.Base;

internal class TitledViewModel : ObservableObject
{
    private string? _controlTitle = "Undefined";

    public string? ControlTitle 
    {
        get => _controlTitle;
        set => SetProperty(ref _controlTitle, value);
    }
}
