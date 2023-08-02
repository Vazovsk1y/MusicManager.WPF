using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;

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

        entityBuilder.Property(e => e.DiscNumber).IsRequired(false);

        entityBuilder.Property(e => e.Number).IsRequired();

        entityBuilder.Property(e => e.Name).IsRequired();
    }
}
