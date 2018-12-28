using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tv7Playlist.Core.Parsers.Xspf
{
    public class XspfParser : IPlaylistParser
    {
        public Task<IReadOnlyCollection<ParsedTrack>> ParseFromStreamAsync(Stream stream)
        {
            var deserializedList = DeserializePlaylist(stream);

            var tracks = deserializedList.TrackList.Select(t => new ParsedTrack(t.Extension.Id, t.Title, t.Location))
                .ToList();

            return Task.FromResult((IReadOnlyCollection<ParsedTrack>) tracks);
        }

        private static Playlist DeserializePlaylist(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Playlist));
            var xmlReaderSettings = GetXmlReaderSettings();

            using (var reader = XmlReader.Create(stream, xmlReaderSettings))
            {
                var deserializedList = (Playlist) serializer.Deserialize(reader);
                return deserializedList;
            }
        }

        private static XmlReaderSettings GetXmlReaderSettings()
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                IgnoreProcessingInstructions = true,
                ValidationFlags = XmlSchemaValidationFlags.None
            };
            return xmlReaderSettings;
        }
    }
}