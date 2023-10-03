using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;

namespace MusicManager.DAL.Configurations;

internal class MovieReleaseLinkConfiguration : IEntityTypeConfiguration<MovieReleaseLink>
{
    public void Configure(EntityTypeBuilder<MovieReleaseLink> builder)
    {
        builder.HasKey(e => new { e.MovieId, e.MovieReleaseId });

        builder.OwnsOne(e => e.ReleaseLinkInfo);
	}
}


