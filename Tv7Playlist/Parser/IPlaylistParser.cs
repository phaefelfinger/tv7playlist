using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tv7Playlist.Parser
{
    public interface IPlaylistParser
    {
        Task<IReadOnlyCollection<ParsedTrack>> ParseFromStream(Stream stream);
    }
}