using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.DAL.Configurations;

internal class SongwriterConfiguration : IEntityTypeConfiguration<Songwriter>
{
    public void Configure(EntityTypeBuilder<Songwriter> builder)
    {
        builder.HasKey(e => e.Id);

        builder
        .Property(e => e.Id)
        .HasConversion(
            valueId => valueId.Value,               // how to store
            value => new SongwriterId(value));      // how to give back from db

        builder.Property(e => e.Name).IsRequired();

        builder.Property(e => e.Surname).IsRequired();

        builder
        .Property(e => e.EntityDirectoryInfo)
        .HasConversion(
            e => e != null ? e.Path : null,
            e => e != null ? EntityDirectoryInfo.Create(e).Value : null)
        .IsRequired(false);

        builder
        .HasMany(e => e.Movies)
        .WithOne()
        .HasForeignKey(e => e.SongwriterId)
        .OnDelete(DeleteBehavior.Cascade);

        builder
        .HasMany(e => e.Compilations)
        .WithOne()
        .HasForeignKey(e => e.SongwriterId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
