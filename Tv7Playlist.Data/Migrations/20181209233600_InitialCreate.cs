using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tv7Playlist.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "PlaylistEntries",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TrackNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistEntries", x => x.Id);
                    table.UniqueConstraint("AK_PlaylistEntries_TrackNumber", x => x.TrackNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "PlaylistEntries");
        }
    }
}