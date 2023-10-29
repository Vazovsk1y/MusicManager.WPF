using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.DAL.Configurations;

internal class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(e => e.Id);

        builder
        .Property(e => e.Id)
        .HasConversion(e => e.Value, value => new MovieId(value));

        builder.Property(e => e.Title).IsRequired();

        builder.OwnsOne(e => e.ProductionInfo);

        builder
            .Property(e => e.AssociatedFolderInfo)
            .HasConversion(
            e => e != null ? e.Path : null,
            e => e != null ? EntityDirectoryInfo.Create(e).Value : null)
        .IsRequired(false);

        builder
            .HasMany(e => e.ReleasesLinks)
            .WithOne(e => e.Movie)
            .HasForeignKey(e => e.MovieId);

        builder
            .HasOne(e => e.Director)
            .WithMany(e => e.Movies);
    }
}