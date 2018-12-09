using System;

namespace Tv7Playlist.Data
{
    public class PlaylistEntry
    {
        public Guid Id { get; set; }

        public int TrackNumber { get; set; }

        public string Name { get; set; }
        
        public string Url { get; set; }
    }
}