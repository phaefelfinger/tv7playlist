using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tv7Playlist.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaylistEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    TrackNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NameOverride = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistEntries", x => x.Id);
                });

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
            migrationBuilder.DropTable(
                name: "PlaylistEntries");
        }
    }
}
