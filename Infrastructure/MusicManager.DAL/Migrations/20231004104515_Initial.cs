﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "directors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    full_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_directors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "discs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    production_info_country = table.Column<string>(type: "TEXT", nullable: true),
                    production_info_year = table.Column<int>(type: "INTEGER", nullable: true),
                    identifier = table.Column<string>(type: "TEXT", nullable: false),
                    associated_folder_info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "songwriters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    last_name = table.Column<string>(type: "TEXT", nullable: false),
                    associated_folder_info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_songwriters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cover",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    disc_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    path = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cover", x => x.id);
                    table.ForeignKey(
                        name: "fk_cover_discs_disc_temp_id",
                        column: x => x.disc_id,
                        principalTable: "discs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movies_releases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies_releases", x => x.id);
                    table.ForeignKey(
                        name: "fk_movies_releases_discs_id",
                        column: x => x.id,
                        principalTable: "discs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    disc_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    disc_number = table.Column<int>(type: "INTEGER", nullable: true),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_songs", x => x.id);
                    table.ForeignKey(
                        name: "fk_songs_discs_disc_temp_id1",
                        column: x => x.disc_id,
                        principalTable: "discs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compilations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    songwriter_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compilations", x => x.id);
                    table.ForeignKey(
                        name: "fk_compilations_discs_id",
                        column: x => x.id,
                        principalTable: "discs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_compilations_songwriters_songwriter_temp_id",
                        column: x => x.songwriter_id,
                        principalTable: "songwriters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    songwriter_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    production_info_country = table.Column<string>(type: "TEXT", nullable: true),
                    production_info_year = table.Column<int>(type: "INTEGER", nullable: true),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    director_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    associated_folder_info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies", x => x.id);
                    table.ForeignKey(
                        name: "fk_movies_directors_director_temp_id",
                        column: x => x.director_id,
                        principalTable: "directors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_movies_songwriters_songwriter_temp_id1",
                        column: x => x.songwriter_id,
                        principalTable: "songwriters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playback_info",
                columns: table => new
                {
                    song_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    executable_file_path = table.Column<string>(type: "TEXT", nullable: false),
                    audio_type = table.Column<string>(type: "TEXT", nullable: false),
                    duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    cue_info_cue_file_path = table.Column<string>(type: "TEXT", nullable: true),
                    cue_info_index00 = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    cue_info_index01 = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    cue_info_song_title_in_cue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playback_info", x => x.song_id);
                    table.ForeignKey(
                        name: "fk_playback_info_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movie_release_links",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    movie_release_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    release_link_info_path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movie_release_links", x => new { x.movie_id, x.movie_release_id });
                    table.ForeignKey(
                        name: "fk_movie_release_links_discs_movie_release_temp_id2",
                        column: x => x.movie_release_id,
                        principalTable: "movies_releases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movie_release_links_movies_movie_temp_id",
                        column: x => x.movie_id,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_compilations_songwriter_id",
                table: "compilations",
                column: "songwriter_id");

            migrationBuilder.CreateIndex(
                name: "ix_cover_disc_id",
                table: "cover",
                column: "disc_id");

            migrationBuilder.CreateIndex(
                name: "ix_movie_release_links_movie_release_id",
                table: "movie_release_links",
                column: "movie_release_id");

            migrationBuilder.CreateIndex(
                name: "ix_movies_director_id",
                table: "movies",
                column: "director_id");

            migrationBuilder.CreateIndex(
                name: "ix_movies_songwriter_id",
                table: "movies",
                column: "songwriter_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_disc_id",
                table: "songs",
                column: "disc_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compilations");

            migrationBuilder.DropTable(
                name: "cover");

            migrationBuilder.DropTable(
                name: "movie_release_links");

            migrationBuilder.DropTable(
                name: "playback_info");

            migrationBuilder.DropTable(
                name: "movies_releases");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "directors");

            migrationBuilder.DropTable(
                name: "songwriters");

            migrationBuilder.DropTable(
                name: "discs");
        }
    }
}
