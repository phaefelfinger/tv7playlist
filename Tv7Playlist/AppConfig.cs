namespace Tv7Playlist
{
    public class AppConfig
    {
        public enum SourceTypeEnum
        {
            M3U,
            XSPF
        }
        
        public string TV7Url { get; set; }
        public SourceTypeEnum SourceType { get; set; }
        public string UdpxyUrl { get; set; }
        public string DownloadFileName { get; set; }
    }
}