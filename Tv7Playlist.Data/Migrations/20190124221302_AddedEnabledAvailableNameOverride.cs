using Microsoft.EntityFrameworkCore.Migrations;

namespace Tv7Playlist.Data.Migrations
{
    public partial class AddedEnabledAvailableNameOverride : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_PlaylistEntries_TrackNumber",
                table: "PlaylistEntries");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "PlaylistEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "PlaylistEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameOverride",
                table: "PlaylistEntries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistEntries_Name",
                table: "PlaylistEntries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistEntries_TrackNumber",
                table: "PlaylistEntries",
                column: "TrackNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlaylistEntries_Name",
                table: "PlaylistEntries");

            migrationBuilder.DropIndex(
                name: "IX_PlaylistEntries_TrackNumber",
                table: "PlaylistEntries");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "PlaylistEntries");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "PlaylistEntries");

            migrationBuilder.DropColumn(
                name: "NameOverride",
                table: "PlaylistEntries");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PlaylistEntries_TrackNumber",
                table: "PlaylistEntries",
                column: "TrackNumber");
        }
    }
}
