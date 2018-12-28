using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;
using Tv7Playlist.Models;

namespace Tv7Playlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppConfig _appConfig;
        private readonly IPlaylistLoader _playlistLoader;

        public HomeController(ILogger<HomeController> logger, IAppConfig appConfig, IPlaylistLoader playlistLoader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _playlistLoader = playlistLoader ?? throw new ArgumentNullException(nameof(playlistLoader));
        }

        public async Task<IActionResult> Index()
        {
            var tracks = await _playlistLoader.LoadPlaylistFromUrl(_appConfig.TV7Url);
            var model = new HomeModel(tracks);
            
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
