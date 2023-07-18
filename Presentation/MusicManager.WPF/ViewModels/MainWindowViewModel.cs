using MusicManager.WPF.ViewModels.Base;

namespace MusicManager.WPF.ViewModels;

internal class MainWindowViewModel : TitledViewModel
{
    #region --Fields--



    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--

    public MainWindowViewModel()
    {
        Title = App.WorkingDirectory;
    }

    #endregion

    #region --Commands--



    #endregion

    #region --Methods--



    #endregion
}
