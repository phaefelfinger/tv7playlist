using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;
using Tv7Playlist.Data;
using Tv7Playlist.Models;

namespace Tv7Playlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppConfig _appConfig;
        private readonly PlaylistContext _playlistContext;

        public HomeController(ILogger<HomeController> logger, IAppConfig appConfig, PlaylistContext playlistContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        public async Task<IActionResult> Index()
        {
            var playlistEntries = await _playlistContext.PlaylistEntries.AsNoTracking().OrderBy(e => e.TrackNumber).ToListAsync();
            var model = new HomeModel(playlistEntries);

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
