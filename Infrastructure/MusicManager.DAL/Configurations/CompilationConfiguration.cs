using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Models;

namespace MusicManager.DAL.Configurations;

internal class CompilationConfiguration : IEntityTypeConfiguration<Compilation>
{
    public void Configure(EntityTypeBuilder<Compilation> entityBuilder)
    {
        entityBuilder.ToTable(MusicManagerDbContext.COMPILATIONS_TABLE_NAME);

        entityBuilder.UseTptMappingStrategy();
    }
}
