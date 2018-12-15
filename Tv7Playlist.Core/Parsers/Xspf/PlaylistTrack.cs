using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Tv7Playlist.Core.Parsers.Xspf
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xspf.org/ns/0/")]
    public class PlaylistTrack
    {
        [XmlElement("title")] public string Title { get; set; }

        [XmlElement("location")] public string Location { get; set; }

        [XmlElement("extension")] public PlaylistTrackExtension Extension { get; set; }
    }
}