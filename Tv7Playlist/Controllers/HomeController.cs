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
        private readonly IAppConfig _appConfig;
        private readonly PlaylistContext _playlistContext;
        private readonly IPlaylistSynchronizer _playlistSynchronizer;

        public HomeController(PlaylistContext playlistContext, IPlaylistSynchronizer playlistSynchronizer, IAppConfig appConfig)
        {
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
            _playlistSynchronizer = playlistSynchronizer ?? throw new ArgumentNullException(nameof(playlistSynchronizer));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlistEntries = await _playlistContext.PlaylistEntries.AsNoTracking().OrderBy(e => e.Position)
                .Select(e => new PlaylistEntryModel(e)).ToListAsync();
            var model = new HomeModel(playlistEntries);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DisableSelectedEntries([FromForm] HomeModel model)
        {
            if (ModelState.IsValid) await UpdateEnabledForItems(model, false);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> EnableSelectedEntries([FromForm] HomeModel model)
        {
            if (ModelState.IsValid) await UpdateEnabledForItems(model, true);

            return Redirect("/");
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

        private async Task UpdateEnabledForItems(HomeModel model, bool isEnabled)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var idsToUpdate = model.PlaylistEntries.Where(e => e.Selected).Select(e => e.Id);
            foreach (var id in idsToUpdate)
            {
                var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
                if (entry == null) continue;

                entry.IsEnabled = isEnabled;
            }

            await _playlistContext.SaveChangesAsync();
        }
    }
}