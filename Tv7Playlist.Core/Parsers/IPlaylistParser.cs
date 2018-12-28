using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tv7Playlist.Core.Parsers
{
    public interface IPlaylistParser
    {
        Task<IReadOnlyCollection<ParsedTrack>> ParseFromStreamAsync(Stream stream);
    }
}