using MusicManager.Domain.Models;
using System.Collections.Generic;

namespace MusicManager.WPF.ViewModels.Entities;

internal partial class MovieReleaseViewModel : DiscViewModel<MovieReleaseViewModel>
{
    public required ICollection<MovieId> MoviesLinks { get; init; }
}