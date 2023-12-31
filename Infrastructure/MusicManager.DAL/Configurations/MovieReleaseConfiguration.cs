﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;

namespace MusicManager.DAL.Configurations;

internal class MovieReleaseConfiguration : IEntityTypeConfiguration<MovieRelease>
{
    public void Configure(EntityTypeBuilder<MovieRelease> entityBuilder)
    {
        entityBuilder.ToTable(MusicManagerDbContext.MOVIES_RELEASES_TABLE_NAME);

		entityBuilder
			.HasMany(e => e.MoviesLinks)
			.WithOne(e => e.MovieRelease)
			.HasForeignKey(e => e.MovieReleaseId);

		entityBuilder.UseTptMappingStrategy();
    }
}
