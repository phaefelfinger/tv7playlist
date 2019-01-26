using Tv7Playlist.Data;

namespace Tv7Playlist.Models
{
    public class PlaylistEntryEditViewModel
    {
        public PlaylistEntryEditViewModel(PlaylistEntry playlistEntry)
        {
            PlaylistEntry = playlistEntry;
        }

        public PlaylistEntry PlaylistEntry { get; }
    }
}