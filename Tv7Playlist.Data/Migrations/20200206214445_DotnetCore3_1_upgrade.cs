using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tv7Playlist.Data.Migrations
{
    public partial class DotnetCore3_1_upgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "PlaylistEntriesTemp",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Position = table.Column<int>(),
                    ChannelNumberImport = table.Column<int>(),
                    ChannelNumberExport = table.Column<int>(),
                    Name = table.Column<string>(nullable: true),
                    EpgMatchName = table.Column<string>(nullable: true),
                    UrlProxy = table.Column<string>(nullable: true),
                    UrlOriginal = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(),
                    IsEnabled = table.Column<bool>(),
                    Created = table.Column<DateTime>(),
                    Modified = table.Column<DateTime>()
                });

            migrationBuilder.Sql("INSERT INTO PlaylistEntriesTemp SELECT * From PlaylistEntries");

            migrationBuilder.DropTable("PlaylistEntries");

            migrationBuilder.CreateTable(
                "PlaylistEntries",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Position = table.Column<int>(),
                    ChannelNumberImport = table.Column<int>(),
                    ChannelNumberExport = table.Column<int>(),
                    Name = table.Column<string>(nullable: true),
                    EpgMatchName = table.Column<string>(nullable: true),
                    UrlProxy = table.Column<string>(nullable: true),
                    UrlOriginal = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(),
                    IsEnabled = table.Column<bool>(),
                    Created = table.Column<DateTime>(),
                    Modified = table.Column<DateTime>()
                },
                constraints: table => { table.PrimaryKey("PK_PlaylistEntries", x => x.Id); });

            migrationBuilder.Sql("INSERT INTO PlaylistEntries SELECT * From PlaylistEntriesTemp");

            migrationBuilder.CreateIndex(
                "IX_PlaylistEntries_ChannelNumberImport",
                "PlaylistEntries",
                "ChannelNumberImport",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_PlaylistEntries_Name",
                "PlaylistEntries",
                "Name");

            migrationBuilder.DropTable("PlaylistEntriesTemp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}