using System;
using System.Collections.Generic;

namespace Tv7Playlist.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
        }

        public HomeModel(List<PlaylistEntryModel> playlistEntries)
        {
            PlaylistEntries = playlistEntries ?? throw new ArgumentNullException(nameof(playlistEntries));
        }

        public List<PlaylistEntryModel> PlaylistEntries { get; set; }
    }
}