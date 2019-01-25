using System.Threading.Tasks;

namespace Tv7Playlist.Core
{
    public interface IPlaylistSynchronizer
    {
        Task SynchronizeAsync();
    }
}