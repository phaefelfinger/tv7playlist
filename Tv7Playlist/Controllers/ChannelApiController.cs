using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Data;

namespace Tv7Playlist.Controllers
{
    [Route("api/channels")]
    [ApiController]
    public class ChannelApiController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlaylistContext _playlistContext;

        public ChannelApiController(ILogger<HomeController> logger, PlaylistContext playlistContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playlistContext = playlistContext ?? throw new ArgumentNullException(nameof(playlistContext));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var playlistEntries =
                await _playlistContext.PlaylistEntries.AsNoTracking().OrderBy(e => e.Position).ToListAsync();
            var result = new {Data = playlistEntries};
            return Ok(result);
        }

        [HttpPut]
        [Route("disable")]
        public async Task<IActionResult> DisableChannels([FromBody] ICollection<Guid> ids)
        {
            if (ids == null) return BadRequest();
            await UpdateEnabledForItems(ids, false);
            return Ok();
        }

        [HttpPut]
        [Route("enable")]
        public async Task<IActionResult> EnableChannels([FromBody] ICollection<Guid> ids)
        {
            if (ids == null) return BadRequest();
            await UpdateEnabledForItems(ids, true);
            return Ok();
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteChannels([FromBody] ICollection<Guid> ids)
        {
            if (ids == null) return BadRequest();

            foreach (var id in ids)
            {
                var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogDebug($"Could not delete! Channel {id} not found");
                    continue;
                }

                _logger.LogInformation($"Deleting channel {id} - {entry.Name}");
                _playlistContext.PlaylistEntries.Remove(entry);
            }

            await _playlistContext.SaveChangesAsync();

            return Ok();
        }

        private async Task UpdateEnabledForItems(IEnumerable<Guid> ids, bool isEnabled)
        {
            foreach (var id in ids)
            {
                var entry = await _playlistContext.PlaylistEntries.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogDebug($"Could not set enabled state! Channel {id} not found");
                    continue;
                }

                _logger.LogInformation($"Setting enabled of channel {id} - {entry.Name} to {isEnabled}");
                entry.IsEnabled = isEnabled;
                entry.Modified = DateTime.Now;
            }

            await _playlistContext.SaveChangesAsync();
        }
    }
}