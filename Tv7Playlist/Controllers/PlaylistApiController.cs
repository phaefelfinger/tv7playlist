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

        public PlaylistApiController(ILogger<HomeController> logger, PlaylistContext playlistContext,
            IPlaylistLoader playlistLoader, IAppConfig appConfig)
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

            var tracks = await _playlistLoader.LoadPlaylistFromUrl(_appConfig.TV7Url);
            await MarkNotAvailableEntriesAsync(tracks);
            await AddOrUpdateEntriesAsync(tracks);
            _logger.LogDebug("Synchronizing playlist completed saving changes...");

            await _playlistContext.SaveChangesAsync();
            _logger.LogDebug("Playlist changes saved successfully...");

            return Ok();
        }

        private async Task AddOrUpdateEntriesAsync(IEnumerable<ParsedTrack> tracks)
        {
            foreach (var track in tracks)
            {
                var entry = await _playlistContext.PlaylistEntries.Where(e => e.TrackNumber == track.Id).FirstOrDefaultAsync();
                if (entry == null)
                {
                    _logger.LogInformation($"Adding playlist entry {track.Id} - {track.Name}");
                    entry = new PlaylistEntry {Id = Guid.NewGuid(), TrackNumber = track.Id, IsEnabled = true};
                    _playlistContext.PlaylistEntries.Add(entry);
                }

                entry.IsAvailable = true;
                entry.Name = track.Name;
                entry.Url = track.Url;
            }
        }

        private async Task MarkNotAvailableEntriesAsync(IReadOnlyCollection<ParsedTrack> tracks)
        {
            var unavailableEntries =
                await _playlistContext.PlaylistEntries.Where(e => tracks.All(t => t.Id != e.TrackNumber)).ToListAsync();
            foreach (var entry in unavailableEntries)
            {
                _logger.LogInformation($"Channel {entry.TrackNumber} - {entry.Name} is no longer available.");
                entry.IsAvailable = false;
            }
        }
    }
}