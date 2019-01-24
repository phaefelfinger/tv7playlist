using System;

namespace Tv7Playlist.Data
{
    public class PlaylistEntry
    {
        public Guid Id { get; set; }

        public int Position { get; set; }
        public int TrackNumber { get; set; }

        public string Name { get; set; }

        public string NameOverride { get; set; }
        
        public string Url { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsEnabled { get; set; }
    }
}