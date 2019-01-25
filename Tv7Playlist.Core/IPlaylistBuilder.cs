using System.IO;
using System.Threading.Tasks;

namespace Tv7Playlist.Core
{
    public interface IPlaylistBuilder
    {
        Task<Stream> GeneratePlaylistAsync();
    }
}