using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;

namespace MusicManager.DAL.Configurations;

internal class PlaybackInfoConfiguration : IEntityTypeConfiguration<PlaybackInfo>
{
    public void Configure(EntityTypeBuilder<PlaybackInfo> entityBuilder)
    {
        entityBuilder.HasKey(e => e.SongId);

        entityBuilder
        .HasOne<Song>()
        .WithOne(e => e.PlaybackInfo)
        .HasForeignKey<PlaybackInfo>(e => e.SongId);

        entityBuilder.Property(e => e.ExecutableType)
            .HasConversion(
            e => e.ToString(),
            e => e.CreateSongFileType().Value
            ).IsRequired();

        entityBuilder.Property(e => e.ExecutableFileFullPath).IsRequired();

        entityBuilder.Property(e => e.CueFilePath).IsRequired(false);

        entityBuilder.Property(e => e.SongDuration).IsRequired();
    }
}
