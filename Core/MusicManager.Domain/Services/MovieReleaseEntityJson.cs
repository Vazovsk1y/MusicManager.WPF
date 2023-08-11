﻿using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;

namespace MusicManager.Domain.Services;

public class MovieReleaseEntityJson : SerializableEntityInfo<MovieRelease>
{
    public string Identifier { get; set; }

    public string? ProductionCountry { get; set; }

    public int? ProductionYear { get; set; }

    public DiscType DiscType { get; set; }
}