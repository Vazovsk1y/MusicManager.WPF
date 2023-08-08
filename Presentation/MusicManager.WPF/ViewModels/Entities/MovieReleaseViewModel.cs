using MusicManager.Domain.Models;
using System.Collections.Generic;

namespace MusicManager.WPF.ViewModels.Entities;

internal class MovieReleaseViewModel : DiscViewModel
{
    public required ICollection<MovieId> MoviesLinks { get; init; }
}