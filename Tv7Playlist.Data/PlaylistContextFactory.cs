using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tv7Playlist.Data
{
    public class PlaylistContextFactory : IDesignTimeDbContextFactory<PlaylistContext>
    {
        public PlaylistContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PlaylistContext>();
            optionsBuilder.UseSqlite("Data Source=designPlaylist.db");

            return new PlaylistContext(optionsBuilder.Options);
        }
    }
}