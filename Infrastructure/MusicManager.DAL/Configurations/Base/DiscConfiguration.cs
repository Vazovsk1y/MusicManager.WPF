using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.DAL.Configurations.Base;

internal class DiscConfiguration : IEntityTypeConfiguration<Disc>
{
    public void Configure(EntityTypeBuilder<Disc> entityBuilder)
    {
        entityBuilder.ToTable(MusicManagerDbContext.DISCS_TABLE_NAME);

        entityBuilder.UseTptMappingStrategy();

        entityBuilder.HasKey(e => e.Id);

        entityBuilder
            .Property(e => e.Id)
            .HasConversion(e => e.Value, value => new DiscId(value));

        entityBuilder.OwnsOne(e => e.ProductionInfo);

        entityBuilder
            .Property(e => e.EntityDirectoryInfo)
            .HasConversion(
            e => e != null ? e.Path : null,
            e => e != null ? EntityDirectoryInfo.Create(e).Value : null)
        .IsRequired(false);

        entityBuilder.Property(e => e.Type)
               .HasConversion(
            e => e.Value,
            e => DiscType.Create(e).Value)
               .IsRequired();

        entityBuilder.Property(e => e.Identifier).IsRequired();

        entityBuilder
            .HasMany(e => e.Covers)
            .WithOne()
            .HasForeignKey(e => e.DiscId);

        entityBuilder
            .HasMany(e => e.Songs)
            .WithOne()
            .HasForeignKey(e => e.DiscId);
    }
}
