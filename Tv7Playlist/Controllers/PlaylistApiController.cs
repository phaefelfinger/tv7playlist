using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;
using Tv7Playlist.Core.Parsers;
using Tv7Playlist.Data;

namespace Tv7Playlist.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistApiController : Controller
    {
        private readonly IAppConfig _appConfig;
        private readonly ILogger<HomeController> _logger;
        private readonly PlaylistContext _playlistContext;
        private readonly IPlaylistLoader _playlistLoader;

        public PlaylistApiController(ILogger<HomeController> logger, PlaylistContext playlistContext, IPlaylistLoader playlistLoader,
            IAppConfig appConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
            _playlistLoader = playlistLoader ?? throw new ArgumentNullException(nameof(playlistLoader));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        [HttpGet]
        [Route("synchronize")]
        public async Task<IActionResult> Synchronize()
        {
            //TODO: Refactor to post method
            _logger.LogDebug("Synchronizing playlist from server...");

            var existingEntries = await _playlistContext.PlaylistEntries.ToDictionaryAsync(e => e.TrackNumber);
            var tracks = await _playlistLoader.LoadPlaylistFromUrl(_appConfig.TV7Url);

            MarkNotAvailableEntries(existingEntries, tracks);
            AddOrUpdateEntries(existingEntries, tracks);

            _logger.LogDebug("Synchronizing playlist completed saving changes...");

            await _playlistContext.SaveChangesAsync();
            _logger.LogDebug("Playlist changes saved successfully...");

            return Ok();
        }

        private void AddOrUpdateEntries(Dictionary<int, PlaylistEntry> existingEntries, IEnumerable<ParsedTrack> tracks)
        {
            foreach (var track in tracks)
            {
                if (!existingEntries.TryGetValue(track.Id, out var entry))
                {
                    _logger.LogInformation($"Adding playlist entry {track.Id} - {track.Name}");
                    entry = new PlaylistEntry
                    {
                        Id = Guid.NewGuid(), Position = track.Id, TrackNumber = track.Id, IsEnabled = true
                    };
                    _playlistContext.PlaylistEntries.Add(entry);
                }
                else
                {
                    _logger.LogInformation($"Updating playlist entry {track.Id} - {track.Name}");
                }

                entry.IsAvailable = true;
                entry.Name = track.Name;
                entry.Url = track.Url;
            }
        }

        private void MarkNotAvailableEntries(Dictionary<int, PlaylistEntry> existingEntries,
            IReadOnlyCollection<ParsedTrack> tracks)
        {
            var unavailableEntries = existingEntries.Where(e => tracks.All(t => t.Id != e.Key)).Select(e => e.Value);
            foreach (var entry in unavailableEntries)
            {
                _logger.LogInformation($"Channel {entry.TrackNumber} - {entry.Name} is no longer available.");
                entry.IsAvailable = false;
            }
        }
    }
}