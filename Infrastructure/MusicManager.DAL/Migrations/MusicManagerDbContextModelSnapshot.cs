﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicManager.DAL;

#nullable disable

namespace MusicManager.DAL.Migrations
{
    [DbContext(typeof(MusicManagerDbContext))]
    partial class MusicManagerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("MovieMovieRelease", b =>
                {
                    b.Property<Guid>("MoviesId")
                        .HasColumnType("TEXT")
                        .HasColumnName("movies_id");

                    b.Property<Guid>("ReleasesId")
                        .HasColumnType("TEXT")
                        .HasColumnName("releases_id");

                    b.HasKey("MoviesId", "ReleasesId")
                        .HasName("pk_movie_movie_release");

                    b.HasIndex("ReleasesId")
                        .HasDatabaseName("ix_movie_movie_release_releases_id");

                    b.ToTable("movie_movie_release", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Common.Disc", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("EntityDirectoryInfo")
                        .HasColumnType("TEXT")
                        .HasColumnName("entity_directory_info");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("identifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.ToTable("discs", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MusicManager.Domain.Entities.Cover", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<Guid>("DiscId")
                        .HasColumnType("TEXT")
                        .HasColumnName("disc_id");

                    b.Property<string>("FullPath")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("full_path");

                    b.HasKey("Id")
                        .HasName("pk_cover");

                    b.HasIndex("DiscId")
                        .HasDatabaseName("ix_cover_disc_id");

                    b.ToTable("cover", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Entities.PlaybackInfo", b =>
                {
                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT")
                        .HasColumnName("song_id");

                    b.Property<string>("ExecutableFileFullPath")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("executable_file_full_path");

                    b.Property<string>("ExecutableType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("executable_type");

                    b.Property<TimeSpan>("SongDuration")
                        .HasColumnType("TEXT")
                        .HasColumnName("song_duration");

                    b.HasKey("SongId")
                        .HasName("pk_playback_info");

                    b.ToTable("playback_info", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("EntityDirectoryInfo")
                        .HasColumnType("TEXT")
                        .HasColumnName("entity_directory_info");

                    b.Property<Guid>("SongwriterId")
                        .HasColumnType("TEXT")
                        .HasColumnName("songwriter_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_movies");

                    b.HasIndex("SongwriterId")
                        .HasDatabaseName("ix_movies_songwriter_id");

                    b.ToTable("movies", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<Guid>("DiscId")
                        .HasColumnType("TEXT")
                        .HasColumnName("disc_id");

                    b.Property<int?>("DiscNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("disc_number");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER")
                        .HasColumnName("order");

                    b.HasKey("Id")
                        .HasName("pk_songs");

                    b.HasIndex("DiscId")
                        .HasDatabaseName("ix_songs_disc_id");

                    b.ToTable("songs", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Songwriter", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("EntityDirectoryInfo")
                        .HasColumnType("TEXT")
                        .HasColumnName("entity_directory_info");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("surname");

                    b.HasKey("Id")
                        .HasName("pk_songwriters");

                    b.ToTable("songwriters", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Compilation", b =>
                {
                    b.HasBaseType("MusicManager.Domain.Common.Disc");

                    b.Property<Guid>("SongwriterId")
                        .HasColumnType("TEXT")
                        .HasColumnName("songwriter_id");

                    b.HasIndex("SongwriterId")
                        .HasDatabaseName("ix_compilations_songwriter_id");

                    b.ToTable("compilations", (string)null);
                });

            modelBuilder.Entity("MusicManager.Domain.Models.MovieRelease", b =>
                {
                    b.HasBaseType("MusicManager.Domain.Common.Disc");

                    b.ToTable("movies_releases", (string)null);
                });

            modelBuilder.Entity("MovieMovieRelease", b =>
                {
                    b.HasOne("MusicManager.Domain.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_movie_movie_release_movies_movies_temp_id");

                    b.HasOne("MusicManager.Domain.Models.MovieRelease", null)
                        .WithMany()
                        .HasForeignKey("ReleasesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_movie_movie_release_discs_releases_temp_id2");
                });

            modelBuilder.Entity("MusicManager.Domain.Common.Disc", b =>
                {
                    b.OwnsOne("MusicManager.Domain.ValueObjects.ProductionInfo", "ProductionInfo", b1 =>
                        {
                            b1.Property<Guid>("DiscId")
                                .HasColumnType("TEXT")
                                .HasColumnName("id");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("production_info_country");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER")
                                .HasColumnName("production_info_year");

                            b1.HasKey("DiscId");

                            b1.ToTable("discs");

                            b1.WithOwner()
                                .HasForeignKey("DiscId")
                                .HasConstraintName("fk_discs_discs_id");
                        });

                    b.Navigation("ProductionInfo");
                });

            modelBuilder.Entity("MusicManager.Domain.Entities.Cover", b =>
                {
                    b.HasOne("MusicManager.Domain.Common.Disc", null)
                        .WithMany("Covers")
                        .HasForeignKey("DiscId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cover_discs_disc_temp_id");
                });

            modelBuilder.Entity("MusicManager.Domain.Entities.PlaybackInfo", b =>
                {
                    b.HasOne("MusicManager.Domain.Models.Song", null)
                        .WithOne("PlaybackInfo")
                        .HasForeignKey("MusicManager.Domain.Entities.PlaybackInfo", "SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_playback_info_songs_song_id");

                    b.OwnsOne("MusicManager.Domain.ValueObjects.CueInfo", "CueInfo", b1 =>
                        {
                            b1.Property<Guid>("PlaybackInfoSongId")
                                .HasColumnType("TEXT")
                                .HasColumnName("song_id");

                            b1.Property<string>("CueFilePath")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("cue_info_cue_file_path");

                            b1.Property<TimeSpan>("Index00")
                                .HasColumnType("TEXT")
                                .HasColumnName("cue_info_index00");

                            b1.Property<TimeSpan>("Index01")
                                .HasColumnType("TEXT")
                                .HasColumnName("cue_info_index01");

                            b1.HasKey("PlaybackInfoSongId");

                            b1.ToTable("playback_info");

                            b1.WithOwner()
                                .HasForeignKey("PlaybackInfoSongId")
                                .HasConstraintName("fk_playback_info_playback_info_song_id");
                        });

                    b.Navigation("CueInfo");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Movie", b =>
                {
                    b.HasOne("MusicManager.Domain.Models.Songwriter", null)
                        .WithMany("Movies")
                        .HasForeignKey("SongwriterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_movies_songwriters_songwriter_temp_id1");

                    b.OwnsOne("MusicManager.Domain.ValueObjects.ProductionInfo", "ProductionInfo", b1 =>
                        {
                            b1.Property<Guid>("MovieId")
                                .HasColumnType("TEXT")
                                .HasColumnName("id");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("production_info_country");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER")
                                .HasColumnName("production_info_year");

                            b1.HasKey("MovieId");

                            b1.ToTable("movies");

                            b1.WithOwner()
                                .HasForeignKey("MovieId")
                                .HasConstraintName("fk_movies_movies_id");
                        });

                    b.OwnsOne("MusicManager.Domain.ValueObjects.DirectorInfo", "DirectorInfo", b1 =>
                        {
                            b1.Property<Guid>("MovieId")
                                .HasColumnType("TEXT")
                                .HasColumnName("id");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("director_info_name");

                            b1.Property<string>("Surname")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("director_info_surname");

                            b1.HasKey("MovieId");

                            b1.ToTable("movies");

                            b1.WithOwner()
                                .HasForeignKey("MovieId")
                                .HasConstraintName("fk_movies_movies_id");
                        });

                    b.Navigation("DirectorInfo");

                    b.Navigation("ProductionInfo");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Song", b =>
                {
                    b.HasOne("MusicManager.Domain.Common.Disc", null)
                        .WithMany("Songs")
                        .HasForeignKey("DiscId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_songs_discs_disc_temp_id1");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Compilation", b =>
                {
                    b.HasOne("MusicManager.Domain.Common.Disc", null)
                        .WithOne()
                        .HasForeignKey("MusicManager.Domain.Models.Compilation", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_compilations_discs_id");

                    b.HasOne("MusicManager.Domain.Models.Songwriter", null)
                        .WithMany("Compilations")
                        .HasForeignKey("SongwriterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_compilations_songwriters_songwriter_temp_id");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.MovieRelease", b =>
                {
                    b.HasOne("MusicManager.Domain.Common.Disc", null)
                        .WithOne()
                        .HasForeignKey("MusicManager.Domain.Models.MovieRelease", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_movies_releases_discs_id");
                });

            modelBuilder.Entity("MusicManager.Domain.Common.Disc", b =>
                {
                    b.Navigation("Covers");

                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Song", b =>
                {
                    b.Navigation("PlaybackInfo");
                });

            modelBuilder.Entity("MusicManager.Domain.Models.Songwriter", b =>
                {
                    b.Navigation("Compilations");

                    b.Navigation("Movies");
                });
#pragma warning restore 612, 618
        }
    }
}
