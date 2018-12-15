using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Tv7Playlist.Core.Parsers.Xspf
{
    /// <remarks>
    ///     <?xml version="1.0" encoding="UTF-8"?>
    ///     <playlist xmlns="http://xspf.org/ns/0/" xmlns:vlc="http://www.videolan.org/vlc/playlist/ns/0/" version="1">
    ///         <title>TV7 Playlist</title>
    ///         <trackList>
    ///             <track>
    ///                 <title>SRF1 HD</title>
    ///                 <location>udp://@239.77.0.77:5000</location>
    ///                 <extension application="http://www.videolan.org/vlc/playlist/0">
    ///                     <vlc:id>1000</vlc:id>
    ///                     <vlc:option>network-caching=1000</vlc:option>
    ///                 </extension>
    ///             </track>
    ///             <track>
    ///                 <title>SRFzwei HD</title>
    ///                 <location>udp://@239.77.0.78:5000</location>
    ///                 <extension application="http://www.videolan.org/vlc/playlist/0">
    ///                     <vlc:id>1005</vlc:id>
    ///                     <vlc:option>network-caching=1000</vlc:option>
    ///                 </extension>
    ///             </track>
    ///         </trackList>
    ///     </playlist>
    ///     ///
    /// </remarks>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xspf.org/ns/0/")]
    [XmlRoot(Namespace = "http://xspf.org/ns/0/", IsNullable = false, ElementName = "playlist")]
    public class Playlist
    {
        [XmlElement("title")] public string Title { get; set; }

        [XmlArrayItem("track", IsNullable = false)]
        [XmlArray("trackList")]
        public PlaylistTrack[] TrackList { get; set; }

        [XmlAttribute("version")] public byte Version { get; set; }
    }
}