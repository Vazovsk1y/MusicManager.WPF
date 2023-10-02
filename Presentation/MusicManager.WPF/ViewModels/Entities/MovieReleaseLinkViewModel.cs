using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieReleaseLinkViewModel : ObservableObject
{
	public required MovieReleaseViewModel MovieRelease { get; set; }

	public required bool IsFolder { get; set; }
}