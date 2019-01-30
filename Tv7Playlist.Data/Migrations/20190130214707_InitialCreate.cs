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
                    ChannelNumberImport = table.Column<int>(nullable: false),
                    ChannelNumberExport = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EpgMatchName = table.Column<string>(nullable: true),
                    UrlProxy = table.Column<string>(nullable: true),
                    UrlOriginal = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistEntries_ChannelNumberImport",
                table: "PlaylistEntries",
                column: "ChannelNumberImport",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistEntries_Name",
                table: "PlaylistEntries",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistEntries");
        }
    }
}
