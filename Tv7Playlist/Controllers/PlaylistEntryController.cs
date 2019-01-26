using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tv7Playlist.Core;
using Tv7Playlist.Data;
using Tv7Playlist.Models;

namespace Tv7Playlist.Controllers
{
    public class PlaylistEntryController : Controller
    {
        private readonly PlaylistContext _playlistContext;

        public PlaylistEntryController(PlaylistContext playlistContext)
        {
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
            if (entry == null) return NotFound();

            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PlaylistEntry updatedEntry)
        //[Bind("PlaylistEntry.Id,PlaylistEntry.Position,PlaylistEntry.TrackNumberOverride,PlaylistEntry.NameOverride,PlaylistEntry.IsEnabled")]
        {
            if (updatedEntry == null) return NotFound();

            if (id != updatedEntry.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
                if (entry == null) return NotFound();

                entry.Position = updatedEntry.Position;
                entry.TrackNumberOverride = updatedEntry.TrackNumberOverride;
                entry.NameOverride = updatedEntry.NameOverride;
                entry.IsEnabled = updatedEntry.IsEnabled;

                await _playlistContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(updatedEntry);
        }
    }
}