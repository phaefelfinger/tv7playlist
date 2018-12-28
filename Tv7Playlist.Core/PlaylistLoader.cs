using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core.Parsers;

namespace Tv7Playlist.Core
{
    public class PlaylistLoader : IPlaylistLoader
    {
        private readonly ILogger<PlaylistLoader> _logger;
        private readonly IPlaylistParser _playlistParser;

        public PlaylistLoader(ILogger<PlaylistLoader> logger, IPlaylistParser playlistParser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistParser = playlistParser ?? throw new ArgumentNullException(nameof(playlistParser));
        }

        public async Task<IReadOnlyCollection<ParsedTrack>> LoadPlaylistFromUrl(string url)
        {
            using (var httpClient = new HttpClient())
            {
                _logger.LogInformation(LoggingEvents.Playlist, "Downloading playlist from {tv7url}", url);
                
                var tv7Response = await httpClient.GetAsync(url);
                if (!tv7Response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(LoggingEvents.PlaylistNotFound, "Could not download playlist from {tv7url}. The StatusCode was: {StatusCode}", url, tv7Response.StatusCode);
                }

                return await ParseTracksFromResponseAsync(tv7Response);
            }
        }
        
        private async Task<IReadOnlyCollection<ParsedTrack>> ParseTracksFromResponseAsync(HttpResponseMessage tv7Response)
        {
            _logger.LogInformation(LoggingEvents.Playlist, "Parse");

            using (var tv7ReadStream = await tv7Response.Content.ReadAsStreamAsync())
            {
                return await _playlistParser.ParseFromStreamAsync(tv7ReadStream);
            }
        }
    }
}