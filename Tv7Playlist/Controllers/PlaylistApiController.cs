using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;
using Tv7Playlist.Data;

namespace Tv7Playlist.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistApiController : Controller
    {
        private const string PlayListContentType = "audio/mpegurl";

        private readonly IAppConfig _appConfig;
        private readonly ILogger<HomeController> _logger;

        private readonly IPlaylistBuilder _playlistBuilder;
        private readonly PlaylistContext _playlistContext;

        public PlaylistApiController(ILogger<HomeController> logger, IPlaylistBuilder playlistBuilder, IAppConfig appConfig, PlaylistContext playlistContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistBuilder = playlistBuilder ?? throw new ArgumentNullException(nameof(playlistBuilder));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        [HttpGet]
        [Route("without-proxy")]
        public async Task<IActionResult> GetPlaylistWithoutProxy()
        {
            return await GetPlaylistInternal(false);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetPlaylist()
        {
            return await GetPlaylistInternal(true);
        }

        [HttpGet]
        [Route("with-proxy")]
        public async Task<IActionResult> GetPlaylistWithProxy()
        {
            return await GetPlaylistInternal(true);
        }

        [HttpPut]
        [Route("disable")]
        public async Task<IActionResult> DisableChannels([FromBody]ICollection<Guid> ids)
        {
            await UpdateEnabledForItems(ids, false);
            return Ok();
        }
        
        [HttpPut]
        [Route("enable")]
        public async Task<IActionResult> EnableChannels([FromBody]ICollection<Guid> ids)
        {
            await UpdateEnabledForItems(ids, true);
            return Ok();
        }
        
        private async Task<IActionResult> GetPlaylistInternal(bool useProxy)
        {
            var playlistStream = await _playlistBuilder.GeneratePlaylistAsync(useProxy);
            var downloadFileName = GetDownloadFileName();

            _logger.LogInformation(LoggingEvents.Playlist, "Sending updated playlist {filename}",
                downloadFileName);

            return new FileStreamResult(playlistStream, PlayListContentType)
            {
                FileDownloadName = downloadFileName
            };
        }

        private string GetDownloadFileName()
        {
            var downloadFileName = _appConfig.DownloadFileName;
            if (string.IsNullOrEmpty(downloadFileName)) downloadFileName = "playlist.m3u";

            return downloadFileName;
        }
        
        private async Task UpdateEnabledForItems(IEnumerable<Guid> ids, bool isEnabled)
        {
            foreach (var id in ids)
            {
                var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
                if (entry == null) continue;

                entry.IsEnabled = isEnabled;
            }

            await _playlistContext.SaveChangesAsync();
        }
    }
}