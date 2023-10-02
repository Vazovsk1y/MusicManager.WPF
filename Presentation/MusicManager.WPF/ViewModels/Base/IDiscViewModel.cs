using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace MusicManager.WPF.ViewModels.Entities;

internal interface IDiscViewModel
{
    int? ProductionYear { get; set; }

    string? ProductionCountry { get; set; }

    DiscType SelectedDiscType { get; set; }

    string Identifier { get; set; }

    DiscId DiscId { get; init; }

    ObservableCollection<SongViewModel> Songs { get; init; }
}
