using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Data;

namespace Tv7Playlist.Core
{
    public class PlaylistBuilder : IPlaylistBuilder
    {
        private readonly ILogger<PlaylistBuilder> _logger;
        private readonly PlaylistContext _playlistContext;

        public PlaylistBuilder(ILogger<PlaylistBuilder> logger, PlaylistContext playlistContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        public async Task<Stream> GeneratePlaylistAsync()
        {
            _logger.LogInformation(LoggingEvents.Playlist, "Building m3u file content");

            var playlistEntries = await _playlistContext.PlaylistEntries.Where(e => e.IsAvailable).Where(e => e.IsEnabled)
                .OrderBy(e => e.Position).AsNoTracking().ToListAsync();


            return await CreatePlaylistStreamAsync(playlistEntries);
        }

        private static async Task<MemoryStream> CreatePlaylistStreamAsync(IEnumerable<PlaylistEntry> playlistEntries)
        {
            var outStream = new MemoryStream();
            var outWriter = new StreamWriter(outStream);

            await outWriter.WriteLineAsync(M3UConstants.ExtFileStartTag);

            foreach (var entry in playlistEntries) await WriteEntryAsync(outWriter, entry);

            outStream.Seek(0, SeekOrigin.Begin);
            
            return outStream;
        }

        private static async Task WriteEntryAsync(TextWriter outWriter, PlaylistEntry entry)
        {
            const int duration = -1;
            var number = entry.TrackNumberOverride != 0 ? entry.TrackNumberOverride : entry.TrackNumber;
            var name = !string.IsNullOrWhiteSpace(entry.NameOverride) ? entry.NameOverride : entry.Name;
            
            var extInfo = $"{M3UConstants.ExtInfStartTag}{duration},{number},{name}";
            await outWriter.WriteLineAsync(extInfo);
            await outWriter.WriteLineAsync(entry.Url);
        }
    }
}