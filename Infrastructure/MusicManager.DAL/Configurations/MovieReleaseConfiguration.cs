using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;

namespace MusicManager.DAL.Configurations;

internal class MovieReleaseConfiguration : IEntityTypeConfiguration<MovieRelease>
{
    public void Configure(EntityTypeBuilder<MovieRelease> entityBuilder)
    {
        entityBuilder.UseTptMappingStrategy();
    }
}


