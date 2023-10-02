using CommunityToolkit.Mvvm.ComponentModel;
using MusicManager.WPF.ViewModels.Base;
using System.Linq;
using System.Reflection;

namespace MusicManager.WPF.ViewModels;

internal class MainWindowViewModel : TitledViewModel
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongwirtersPanelViewModel SongwirtersPanelViewModel { get; }

    public MoviesPanelViewModel MoviesPanelViewModel { get; }

    public DiscsPanelViewModel DiscsPanelViewModel { get; }

    public SongsPanelViewModel SongsPanelViewModel { get; }

    public UserConfigViewModel SettingsViewModel { get; }

    #endregion

    #region --Constructors--

    public MainWindowViewModel()
    {

    }

    public MainWindowViewModel(
        SongwirtersPanelViewModel songwirtersPanelViewModel,
        MoviesPanelViewModel moviesPanelViewModel,
        DiscsPanelViewModel discsPanelViewModel,
        SongsPanelViewModel songsPanelViewModel,
        UserConfigViewModel settingsViewModel)
    {
        SongwirtersPanelViewModel = songwirtersPanelViewModel;
        MoviesPanelViewModel = moviesPanelViewModel;
        DiscsPanelViewModel = discsPanelViewModel;
        SongsPanelViewModel = songsPanelViewModel;
        SettingsViewModel = settingsViewModel;
        ControlTitle = App.Name;

        ActivateAllRecipients();
    }

    #endregion

    #region --Commands--



    #endregion

    #region --Methods--

    private void ActivateAllRecipients()
    {
        var type = GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.GetValue(this) is ObservableRecipient observableRecipient)
            {
                observableRecipient.IsActive = true;
            }
        }
    }

    #endregion
}
