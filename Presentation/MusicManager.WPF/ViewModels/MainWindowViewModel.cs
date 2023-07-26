using MusicManager.WPF.ViewModels.Base;

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

    #endregion

    #region --Constructors--

    public MainWindowViewModel()
    {

    }

    public MainWindowViewModel(
        SongwirtersPanelViewModel songwirtersPanelViewModel, 
        MoviesPanelViewModel moviesPanelViewModel, 
        DiscsPanelViewModel discsPanelViewModel, 
        SongsPanelViewModel songsPanelViewModel) 
    {
        SongwirtersPanelViewModel = songwirtersPanelViewModel;
        MoviesPanelViewModel = moviesPanelViewModel;
        DiscsPanelViewModel = discsPanelViewModel;
        SongsPanelViewModel = songsPanelViewModel;
        ControlTitle = App.Name;

        SongwirtersPanelViewModel.IsActive = true;
    }

    #endregion

    #region --Commands--



    #endregion

    #region --Methods--



    #endregion
}
