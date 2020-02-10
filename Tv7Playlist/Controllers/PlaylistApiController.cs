using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;

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

        public PlaylistApiController(ILogger<HomeController> logger, IPlaylistBuilder playlistBuilder,
            IAppConfig appConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistBuilder = playlistBuilder ?? throw new ArgumentNullException(nameof(playlistBuilder));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
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
    }
}