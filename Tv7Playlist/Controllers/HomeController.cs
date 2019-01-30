using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tv7Playlist.Core;
using Tv7Playlist.Data;
using Tv7Playlist.Models;

namespace Tv7Playlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlaylistContext _playlistContext;
        private readonly IPlaylistSynchronizer _playlistSynchronizer;
        private readonly IAppConfig _appConfig;

        public HomeController(PlaylistContext playlistContext, IPlaylistSynchronizer playlistSynchronizer, IAppConfig appConfig)
        {
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
            _playlistSynchronizer = playlistSynchronizer ?? throw new ArgumentNullException(nameof(playlistSynchronizer));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlistEntries = await _playlistContext.PlaylistEntries.AsNoTracking().OrderBy(e => e.Position).ToListAsync();
            var model = new HomeModel(playlistEntries);

            return View(model);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier};

            return View(errorViewModel);
        }

        [HttpGet]
        [Route("synchronize")]
        public IActionResult Synchronize()
        {
            var homeSynchronizeModel = new HomeSynchronizeModel(_appConfig.TV7Url);
            return View(homeSynchronizeModel);
        }

        [HttpPost]
        [Route("synchronize")]
        public async Task<IActionResult> Synchronize(bool ok)
        {
            await _playlistSynchronizer.SynchronizeAsync();
            
            return RedirectToAction("Index", "Home");
        }
    }
}