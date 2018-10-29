using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace tv7playlist.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private const string PlayListContentType = "audio/mpegurl";

        private static readonly Regex MultiCastRegex = new Regex(@"(udp\:\/\/@)([0-9.:]+)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private readonly string _downloadFileName;
        private readonly string _proxyUrl;

        private readonly string _tv7Url;

        public PlayListController(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var appConfig = configuration.Get<AppConfig>();

            _tv7Url = GetTv7Url(appConfig);
            _proxyUrl = GetProxyUrl(appConfig);
            _downloadFileName = GetDownloadFileName(appConfig);
        }

        [HttpGet]
        public async Task<FileStreamResult> Get()
        {
            using (var httpClient = new HttpClient())
            {
                var tv7Response = await httpClient.GetAsync(_tv7Url);

                var modifiedPlaylist = await BuildProxyPlaylist(tv7Response);

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

            using (var tv7ReadStream = await tv7Response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(tv7ReadStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    line = MultiCastRegex.Replace(line, $"{_proxyUrl}/$2/");
                    outWriter.WriteLine(line);
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