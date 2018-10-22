using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace tv7playlist.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private const string Tv7OriginUrl = "https://api.init7.net/tvchannels.m3u";
        private const string UdpxyRootUrl = "http://tv1.haefelfinger.net:4022/udp";

        private static readonly Regex MulticastRegex = new Regex(@"(udp\:\/\/@)([0-9.:]+)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        [HttpGet]
        public async Task<FileStreamResult> Get()
        {
            var httpClient = new HttpClient();
            var tv7Response = await httpClient.GetAsync(Tv7OriginUrl);


            var outStream = new MemoryStream();
            var outWriter = new StreamWriter(outStream);

            using (var tv7ReadStream = await tv7Response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(tv7ReadStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    line = MulticastRegex.Replace(line, $"{UdpxyRootUrl}/$2/");
                    outWriter.WriteLine(line);
                }

                await outWriter.FlushAsync();
            }

            outStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(outStream, "audio/mpegurl") {FileDownloadName = "PlaylistTV7udpxy.m3u"};
        }
    }
}