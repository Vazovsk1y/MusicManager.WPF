using MusicManager.Domain.Models;

namespace MusicManager.WPF.ViewModels.Entities;

internal class CompilationViewModel : DiscViewModel
{
	public required SongwriterId SongwriterId { get; init; }
}
