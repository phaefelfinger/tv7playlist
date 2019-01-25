using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tv7Playlist.Data;
using Tv7Playlist.Models;

namespace Tv7Playlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlaylistContext _playlistContext;

        public HomeController(PlaylistContext playlistContext)
        {
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlistEntries = await _playlistContext.PlaylistEntries.AsNoTracking().OrderBy(e => e.TrackNumber).ToListAsync();
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
    }
}