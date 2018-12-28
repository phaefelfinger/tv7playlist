using System.Collections.Generic;
using System.Threading.Tasks;
using Tv7Playlist.Core.Parsers;

namespace Tv7Playlist.Core
{
    public interface IPlaylistLoader
    {
        Task<IReadOnlyCollection<ParsedTrack>> LoadPlaylistFromUrl(string url);
    }
}