using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core.Parsers;
using Tv7Playlist.Data;

namespace Tv7Playlist.Core
{
    public class PlaylistSynchronizer : IPlaylistSynchronizer
    {
        private static readonly Regex MultiCastRegex = new Regex(@"(udp\:\/\/@)([0-9.:]+)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private readonly IAppConfig _appConfig;
        private readonly ILogger<PlaylistSynchronizer> _logger;
        private readonly PlaylistContext _playlistContext;
        private readonly IPlaylistLoader _playlistLoader;
        private readonly string _proxyUrl;

        public PlaylistSynchronizer(ILogger<PlaylistSynchronizer> logger, IAppConfig appConfig, IPlaylistLoader playlistLoader,
            PlaylistContext playlistContext)
        {
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistLoader = playlistLoader ?? throw new ArgumentNullException(nameof(playlistLoader));
            _proxyUrl = GetProxyUrl();
        }

        public async Task SynchronizeAsync()
        {
            _logger.LogDebug(LoggingEvents.Synchronize, "Synchronizing playlist from server...");

            var tv7Url = GetTv7SourceUrl();
            var existingEntries = await _playlistContext.PlaylistEntries.ToDictionaryAsync(e => e.TrackNumber);
            var tracks = await _playlistLoader.LoadPlaylistFromUrl(tv7Url);

            MarkNotAvailableEntries(existingEntries, tracks);
            AddOrUpdateEntries(existingEntries, tracks);

            _logger.LogDebug("Synchronizing playlist completed saving changes...");

            await _playlistContext.SaveChangesAsync();
            _logger.LogDebug("Playlist changes saved successfully...");
        }

        private void AddOrUpdateEntries(IReadOnlyDictionary<int, PlaylistEntry> existingEntries, IEnumerable<ParsedTrack> tracks)
        {
            foreach (var track in tracks)
            {
                if (!existingEntries.TryGetValue(track.Id, out var entry))
                {
                    _logger.LogInformation(LoggingEvents.Synchronize, $"Adding playlist entry {track.Id} - {track.Name}");
                    entry = new PlaylistEntry
                    {
                        Id = Guid.NewGuid(), Position = track.Id, TrackNumber = track.Id, IsEnabled = true
                    };
                    _playlistContext.PlaylistEntries.Add(entry);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Synchronize, $"Updating playlist entry {track.Id} - {track.Name}");
                }

                entry.IsAvailable = true;
                entry.Name = track.Name;
                entry.UrlOriginal = track.Url;
                entry.Url = BuildUrl(entry);
            }
        }

        private string BuildUrl(PlaylistEntry entry)
        {
            return MultiCastRegex.Replace(entry.UrlOriginal, $"{_proxyUrl}/$2/");
        }

        private string GetProxyUrl()
        {
            var proxyUrl = _appConfig.UdpxyUrl;

            if (string.IsNullOrWhiteSpace(proxyUrl))
            {
                _logger.LogInformation(LoggingEvents.Synchronize, "No proxy URL set in the application config. Using original URLs");
                return string.Empty;
            }

            if (!Uri.IsWellFormedUriString(proxyUrl, UriKind.Absolute))
            {
                var message = $"The proxyUrl is set to {proxyUrl}. This is not a well formed uri!";
                _logger.LogError(LoggingEvents.Synchronize, message);
                throw new ApplicationException(message);
            }

            return proxyUrl;
        }

        private string GetTv7SourceUrl()
        {
            var tv7Url = _appConfig.TV7Url;

            if (!Uri.IsWellFormedUriString(tv7Url, UriKind.Absolute))
            {
                var message = $"The TV7Url is set to {tv7Url}. This is not a well formed uri!";
                _logger.LogError(LoggingEvents.Synchronize, message);
                throw new ApplicationException(message);
            }

            return tv7Url;
        }

        private void MarkNotAvailableEntries(Dictionary<int, PlaylistEntry> existingEntries,
            IReadOnlyCollection<ParsedTrack> tracks)
        {
            var unavailableEntries = existingEntries.Where(e => tracks.All(t => t.Id != e.Key)).Select(e => e.Value);
            foreach (var entry in unavailableEntries)
            {
                _logger.LogInformation(LoggingEvents.Synchronize, $"Channel {entry.TrackNumber} - {entry.Name} is no longer available.");
                entry.IsAvailable = false;
            }
        }
    }
}