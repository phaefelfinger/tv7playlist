using System;
using Tv7Playlist.Data;

namespace Tv7Playlist.Models
{
    public class PlaylistEntryModel
    {
        public PlaylistEntryModel()
        {
        }

        public PlaylistEntryModel(PlaylistEntry entry)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            Id = entry.Id;
        }

        public Guid Id { get; set; }

        public PlaylistEntry Entry { get; set; }

        public bool Selected { get; set; }
    }
}