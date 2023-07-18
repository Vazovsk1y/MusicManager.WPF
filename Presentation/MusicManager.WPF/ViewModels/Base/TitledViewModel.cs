using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.ViewModels.Base;

internal class TitledViewModel : ObservableObject
{
    private string? _title = "Undefined";

    public string? Title 
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
