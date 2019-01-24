using System;
using System.Collections.Generic;
using Tv7Playlist.Data;

namespace Tv7Playlist.Models
{
    public class HomeModel
    {
        public HomeModel(List<PlaylistEntry> playlistEntries)
        {
            PlaylistEntries = playlistEntries ?? throw new ArgumentNullException(nameof(playlistEntries));
        }

        public List<PlaylistEntry> PlaylistEntries { get; }
    }
}