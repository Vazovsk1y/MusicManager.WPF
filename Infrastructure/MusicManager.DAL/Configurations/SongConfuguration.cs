using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.DAL.Configurations;

internal class SongConfuguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> entityBuilder)
    {
        entityBuilder.HasKey(e => e.Id);

        entityBuilder
            .Property(e => e.Id)
            .HasConversion(
            e => e.Value, 
            value => new SongId(value));

        entityBuilder
           .Property(e => e.DiscNumber)
           .HasConversion(
            e => e != null ? e.Digit : (int?)null,
            value => value == null ? null : DiscNumber.Create((int)value).Value
            )
           .IsRequired(false);

        entityBuilder.Property(e => e.Order).IsRequired();

        entityBuilder.Property(e => e.Title).IsRequired();
    }
}
