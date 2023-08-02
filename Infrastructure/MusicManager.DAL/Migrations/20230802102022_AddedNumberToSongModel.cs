using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedNumberToSongModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "number",
                table: "songs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "number",
                table: "songs");
        }
    }
}
