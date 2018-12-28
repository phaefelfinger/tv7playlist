using System;

namespace Tv7Playlist
{
    public class AppConfig
    {
        public enum SourceTypeEnum
        {
            M3U,
            Xspf
        }

        private string _tv7Url;
        private string _udpxyUrl;

        public string TV7Url
        {
            get => _tv7Url;
            set
            {
                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    throw new ApplicationException($"The TV7Url is set to {value}. This is not a well formed uri!");
                _tv7Url = value;
            }
        }

        public SourceTypeEnum SourceType { get; set; }

        public string UdpxyUrl
        {
            get => _udpxyUrl;
            set
            {
                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    throw new ApplicationException($"The proxyUrl is set to {value}. This is not a well formed uri!");
                _udpxyUrl = value;
            }
        }

        public string DownloadFileName { get; set; }
    }
}