using CodeHorizon.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeHorizon.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarksController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBookmarks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if(!userId.HasValue)
            {
                return Unauthorized();
            }

            var bookmarks = await _bookmarkService.GetUserBookmarksAsync(userId.Value, page, pageSize);

            return Ok(bookmarks);
        }

        [HttpPost("{snippetId}")]
        public async Task<IActionResult> AddBookmark(Guid snippetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }
                var bookmark = await _bookmarkService.AddBookmarkAsync(userId.Value, snippetId);
                return Ok(bookmark);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{snippetId}")]
        public async Task<IActionResult> RemoveBookmark(Guid snippetId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }
                await _bookmarkService.RemoveBookmarkAsync(userId.Value, snippetId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check/{snippetId}")]
        public async Task<IActionResult> IsBookmarked(Guid snippetId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }
            var isBookmarked = await _bookmarkService.IsBookmarkedAsync(userId.Value, snippetId);
            return Ok(new { isBookmarked });
        }
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}
