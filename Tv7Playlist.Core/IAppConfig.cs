namespace Tv7Playlist.Core
{
    public interface IAppConfig
    {
        string TV7Url { get; set; }

        SourceTypeEnum SourceType { get; set; }

        string UdpxyUrl { get; set; }

        string DownloadFileName { get; set; }

        string SqLiteConnectionString { get; set; }
    }
}