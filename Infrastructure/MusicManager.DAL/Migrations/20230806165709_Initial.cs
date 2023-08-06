using System;
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
                name: "discs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    entity_directory_info = table.Column<string>(type: "TEXT", nullable: true),
                    production_info_country = table.Column<string>(type: "TEXT", nullable: true),
                    production_info_year = table.Column<int>(type: "INTEGER", nullable: true),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    identifier = table.Column<string>(type: "TEXT", nullable: false)
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
                    surname = table.Column<string>(type: "TEXT", nullable: false),
                    entity_directory_info = table.Column<string>(type: "TEXT", nullable: true)
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
                    full_path = table.Column<string>(type: "TEXT", nullable: false)
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
                    disc_number = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    number = table.Column<int>(type: "INTEGER", nullable: false)
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
                    director_info_surname = table.Column<string>(type: "TEXT", nullable: true),
                    director_info_name = table.Column<string>(type: "TEXT", nullable: true),
                    entity_directory_info = table.Column<string>(type: "TEXT", nullable: true),
                    title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies", x => x.id);
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
                    executable_file_full_path = table.Column<string>(type: "TEXT", nullable: false),
                    cue_info_cue_file_path = table.Column<string>(type: "TEXT", nullable: true),
                    cue_info_index00 = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    cue_info_index01 = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    executable_type = table.Column<string>(type: "TEXT", nullable: false),
                    song_duration = table.Column<TimeSpan>(type: "TEXT", nullable: false)
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
                name: "movie_movie_release",
                columns: table => new
                {
                    movies_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    releases_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movie_movie_release", x => new { x.movies_id, x.releases_id });
                    table.ForeignKey(
                        name: "fk_movie_movie_release_discs_releases_temp_id2",
                        column: x => x.releases_id,
                        principalTable: "movies_releases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movie_movie_release_movies_movies_temp_id",
                        column: x => x.movies_id,
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
                name: "ix_movie_movie_release_releases_id",
                table: "movie_movie_release",
                column: "releases_id");

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
                name: "movie_movie_release");

            migrationBuilder.DropTable(
                name: "playback_info");

            migrationBuilder.DropTable(
                name: "movies_releases");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "songwriters");

            migrationBuilder.DropTable(
                name: "discs");
        }
    }
}
