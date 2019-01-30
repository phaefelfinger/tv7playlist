using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public async Task<Stream> GeneratePlaylistAsync(bool useProxy)
        {
            _logger.LogInformation(LoggingEvents.Playlist, "Building m3u file content");

            var playlistEntries = await _playlistContext.PlaylistEntries.Where(e => e.IsAvailable).Where(e => e.IsEnabled)
                .OrderBy(e => e.Position).AsNoTracking().ToListAsync();

            return await CreatePlaylistStreamAsync(playlistEntries, useProxy);
        }

        private static async Task<MemoryStream> CreatePlaylistStreamAsync(IEnumerable<PlaylistEntry> playlistEntries, bool useProxy)
        {
            var outStream = new MemoryStream();
            var outWriter = new StreamWriter(outStream);

            await outWriter.WriteLineAsync(M3UConstants.ExtFileStartTag);

            foreach (var entry in playlistEntries) await WriteM3UEntryAsync(outWriter, entry, useProxy);

            await outWriter.FlushAsync();
            
            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        private static async Task WriteM3UEntryAsync(TextWriter outWriter, PlaylistEntry entry, bool useProxy)
        {
            await WriteExtInfoLineAsync(outWriter, entry);
            await WriteUrlAsync(outWriter, entry, useProxy);
        }

        private static async Task WriteUrlAsync(TextWriter outWriter, PlaylistEntry entry, bool useProxy)
        {
            if (useProxy)
                await outWriter.WriteLineAsync(entry.UrlProxy);
            else
                await outWriter.WriteLineAsync(entry.UrlOriginal);
        }

        private static async Task WriteExtInfoLineAsync(TextWriter outWriter, PlaylistEntry entry)
        {
            // telly default tags from 1.1 dev code:
            // - NameKey # Parsed from the end of the exif by default
            // - ChannelNumberKey = "tvg-chno"                          
            // - LogoKey = "tvg-logo"
            // - EPGMatchKey = "tvg-id"
            //#EXTINF:-1 tvg-chno="34" tvg-id="SRF1HD" tvg-logo="http://<url to image file with logo>",test
            var exInfoBuilder = new StringBuilder();
            exInfoBuilder.Append(M3UConstants.ExtInfStartTag);

            const int duration = -1;
            exInfoBuilder.Append(duration);

            AddTag(exInfoBuilder, "tvg-chno", entry.ChannelNumberExport.ToString());
            AddTag(exInfoBuilder, "tvg-id", entry.EpgMatchName);

            if (!string.IsNullOrWhiteSpace(entry.LogoUrl)) AddTag(exInfoBuilder, "tvg-logo", entry.LogoUrl);

            exInfoBuilder.Append(",");
            exInfoBuilder.Append(entry.Name);

            await outWriter.WriteLineAsync(exInfoBuilder.ToString());
        }

        private static void AddTag(StringBuilder builder, string name, string value)
        {
            builder.Append(" ");
            builder.Append(name);
            builder.Append("=\"");
            builder.Append(value);
            builder.Append("\"");
        }
    }
}