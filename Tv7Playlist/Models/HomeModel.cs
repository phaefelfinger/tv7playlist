using System;
using System.Collections.Generic;
using Tv7Playlist.Core.Parsers;

namespace Tv7Playlist.Models
{
    public class HomeModel
    {
        public IReadOnlyCollection<ParsedTrack> Tracks { get; }

        public HomeModel(IReadOnlyCollection<ParsedTrack> tracks)
        {
            Tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));
        }
    }
}