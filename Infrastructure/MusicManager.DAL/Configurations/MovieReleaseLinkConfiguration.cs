using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;

namespace MusicManager.DAL.Configurations;

internal class MovieReleaseLinkConfiguration : IEntityTypeConfiguration<MovieReleaseLink>
{
    public void Configure(EntityTypeBuilder<MovieReleaseLink> builder)
    {
        builder.HasKey(e => new { e.MovieId, e.DiscId });

        builder.OwnsOne(e => e.ReleaseLink);

        builder.Property(e => e.DiscId).HasConversion(e => e.Value, e => new Domain.Common.DiscId(e));
    }
}


