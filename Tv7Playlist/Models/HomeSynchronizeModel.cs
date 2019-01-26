namespace Tv7Playlist.Models
{
    public class HomeSynchronizeModel
    {
        public HomeSynchronizeModel(string synchronizationUrl)
        {
            SynchronizationUrl = synchronizationUrl;
        }

        public string SynchronizationUrl { get; set; }
    }
}