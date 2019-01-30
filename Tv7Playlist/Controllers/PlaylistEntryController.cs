using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tv7Playlist.Data;

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
                entry.ChannelNumberExport = updatedEntry.ChannelNumberExport;
                entry.EpgMatchName = updatedEntry.EpgMatchName;
                entry.IsEnabled = updatedEntry.IsEnabled;
                entry.LogoUrl = updatedEntry.LogoUrl;
                entry.Modified = DateTime.Now;

                await _playlistContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(updatedEntry);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleEnabled(Guid? id)
        {
            var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
            if (entry == null) return NotFound();

            entry.IsEnabled = !entry.IsEnabled;
            
            await _playlistContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
            if (entry == null) return NotFound();

            return View(entry);
        }

        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null) return NotFound();

            var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _playlistContext.PlaylistEntries.Remove(entry);

            await _playlistContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}