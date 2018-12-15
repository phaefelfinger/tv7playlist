using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Tv7Playlist.Core.Parsers.Xspf
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xspf.org/ns/0/")]
    public class PlaylistTrackExtension
    {
        [XmlElement("id", Namespace = "http://www.videolan.org/vlc/playlist/ns/0/")]
        public ushort Id { get; set; }

        [XmlElement("option", Namespace = "http://www.videolan.org/vlc/playlist/ns/0/")]
        public string Option { get; set; }

        [XmlAttribute("application")] public string Application { get; set; }
    }
}