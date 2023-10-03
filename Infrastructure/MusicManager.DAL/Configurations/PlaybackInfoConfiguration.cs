using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Enums;
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

        entityBuilder.Property(e => e.AudioType)
            .HasConversion(
            e => e.ToString(),
            e => Enum.Parse<AudioType>(e)
            ).IsRequired();

        entityBuilder.Property(e => e.ExecutableFilePath).IsRequired();

        entityBuilder.OwnsOne(e => e.CueInfo);

        entityBuilder.Property(e => e.Duration).IsRequired();
    }
}
