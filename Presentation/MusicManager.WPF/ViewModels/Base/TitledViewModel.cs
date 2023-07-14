namespace MusicManager.WPF.ViewModels.Base;

internal class TitledViewModel : ViewModel
{
    private string? _title = "Undefined";

    public string? Title 
    {
        get => _title;
        set => Set(ref _title, value);
    }
}
