using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;

namespace MusicManager.DAL.Configurations;

internal class CoverConfiguration : IEntityTypeConfiguration<Cover>
{
    public void Configure(EntityTypeBuilder<Cover> entityBuilder)
    {
        entityBuilder.HasKey(e => e.Id);

        entityBuilder
            .Property(e => e.Id)
            .HasConversion(
            e => e.Value, 
            value => new CoverId(value));

        entityBuilder.Property(e => e.Path).IsRequired();
    }
}
