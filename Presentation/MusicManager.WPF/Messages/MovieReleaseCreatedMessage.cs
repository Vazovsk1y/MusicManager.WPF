using MusicManager.Domain.Models;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.WPF.ViewModels.Entities;
using System.Collections.Generic;

namespace MusicManager.WPF.Messages;

internal record MovieReleaseCreatedMessage(MovieReleaseViewModel MovieReleaseViewModel, IEnumerable<MovieLinkDTO> MoviesLinks);
