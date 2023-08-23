using MusicManager.Domain.Models;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class CompilationViewModel : 
    DiscViewModel<CompilationViewModel>
{
    public required SongwriterId SongwriterId { get; init; }
}
