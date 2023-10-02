using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;

namespace MusicManager.DAL.Configurations;

internal class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.HasKey(e => e.Id);

        builder
        .Property(e => e.Id)
        .HasConversion(e => e.Value, value => new DirectorId(value));
    }
}
