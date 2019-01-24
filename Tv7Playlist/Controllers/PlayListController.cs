using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;

namespace Tv7Playlist.Controllers
{
    [Route("api/playlist-old")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private const string PlayListContentType = "audio/mpegurl";

        /// <summary>
        ///     This is the regex used to build up the proxy url.
        ///     The first part (udp://@) is ignored while generating the final url
        ///     The multicast address is expected to be a ip-address with a port and is reused
        /// </summary>
        private static readonly Regex MultiCastRegex = new Regex(@"(udp\:\/\/@)([0-9.:]+)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private readonly string _downloadFileName;
        private readonly ILogger<PlayListController> _logger;
        private readonly string _proxyUrl;

        private readonly string _tv7Url;

        public PlayListController(ILogger<PlayListController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var appConfig = configuration.Get<AppConfig>();

            _tv7Url = GetTv7Url(appConfig);
            _proxyUrl = GetProxyUrl(appConfig);
            _downloadFileName = GetDownloadFileName(appConfig);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            using (var httpClient = new HttpClient())
            {
                _logger.LogInformation(LoggingEvents.Playlist, "Downloading playlist from {tv7url}", _tv7Url);
                var tv7Response = await httpClient.GetAsync(_tv7Url);

                if (!tv7Response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(LoggingEvents.PlaylistNotFound,
                        "Could not download playlist from {tv7url}. The StatusCode was: {StatusCode}", _tv7Url,
                        tv7Response.StatusCode);
                    return StatusCode((int) tv7Response.StatusCode);
                }

                var modifiedPlaylist = await BuildProxyPlaylist(tv7Response);

                _logger.LogInformation(LoggingEvents.Playlist, "Sending updated playlist {filename}",
                    _downloadFileName);

                return new FileStreamResult(modifiedPlaylist, PlayListContentType)
                {
                    FileDownloadName = _downloadFileName
                };
            }
        }

        private async Task<MemoryStream> BuildProxyPlaylist(HttpResponseMessage tv7Response)
        {
            var outStream = new MemoryStream();
            var outWriter = new StreamWriter(outStream);
            _logger.LogInformation(LoggingEvents.Playlist, "Building m3u file content");

            using (var tv7ReadStream = await tv7Response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(tv7ReadStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var proxyLine = MultiCastRegex.Replace(line, $"{_proxyUrl}/$2/");
                    outWriter.WriteLine(proxyLine);
                    _logger.LogDebug(LoggingEvents.Playlist, "Transformed {src} to {dst}", line, proxyLine);
                }

                await outWriter.FlushAsync();
            }

            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        private string GetTv7Url(AppConfig configuration)
        {
            var tv7Url = configuration.TV7Url;
            if (!Uri.IsWellFormedUriString(tv7Url, UriKind.Absolute))
                throw new ApplicationException($"The TV7Url is set to {tv7Url}. This is not a well formed uri!");
            return tv7Url;
        }

        private string GetProxyUrl(AppConfig configuration)
        {
            var proxyUrl = configuration.UdpxyUrl;
            if (!Uri.IsWellFormedUriString(proxyUrl, UriKind.Absolute))
                throw new ApplicationException($"The proxyUrl is set to {proxyUrl}. This is not a well formed uri!");

            return proxyUrl;
        }

        private string GetDownloadFileName(AppConfig configuration)
        {
            var downloadFileName = configuration.DownloadFileName;
            if (string.IsNullOrEmpty(downloadFileName)) downloadFileName = "playlist.m3u";

            return downloadFileName;
        }
    }
}