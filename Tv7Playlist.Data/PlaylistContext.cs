using Microsoft.EntityFrameworkCore;

namespace Tv7Playlist.Data
{
    public class PlaylistContext : DbContext
    {
        public PlaylistContext(DbContextOptions<PlaylistContext> options)
            : base(options)
        {
        }

        public DbSet<PlaylistEntry> PlaylistEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityTypeBuilder = modelBuilder.Entity<PlaylistEntry>();
            entityTypeBuilder.HasKey(e => e.Id);
            entityTypeBuilder.HasIndex(e => e.TrackNumber).IsUnique();
            entityTypeBuilder.HasIndex(e => e.Name);
        }
    }
}