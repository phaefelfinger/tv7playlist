using System;

namespace Tv7Playlist.Data
{
    public class PlaylistEntry
    {
        public Guid Id { get; set; }

        public int Position { get; set; }

        public int ChannelNumberImport { get; set; }

        public int ChannelNumberExport { get; set; }

        public string Name { get; set; }

        public string EpgMatchName { get; set; }

        public string UrlProxy { get; set; }

        public string UrlOriginal { get; set; }

        public string LogoUrl { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}