using MusicManager.Domain.Common;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.Generic;

namespace MusicManager.WPF.Messages;

internal record SongCreatedMessage(DiscId DiscId, IEnumerable<SongViewModel> SongsViewsModels);
