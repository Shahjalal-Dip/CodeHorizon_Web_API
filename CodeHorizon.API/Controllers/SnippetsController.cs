using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeHorizon.Application.DTOs.Snippet;
using CodeHorizon.Application.Interfaces;

namespace CodeHorizon.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnippetService _snippetService;

        public SnippetsController(ISnippetService snippetService)
        {
            _snippetService = snippetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? language = null,
            [FromQuery] string? search = null,
            [FromQuery] string? tag = null,
            [FromQuery] string? sortBy = "created",
            [FromQuery] string? sortOrder = "desc",
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null) 
        {
            var userId = GetCurrentUserId();
            
            var filter = new SnippetFilterDto
            {
                Language = language,
                Search = search,
                Tag = tag,
                SortBy = sortBy,
                SortOrder = sortOrder,
                FromDate = fromDate,
                ToDate = toDate,
                IsPublic = true, // Only public snippets for unauthenticated users
            };

            var result = await _snippetService.GetAllFilteredAsync(filter, page, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("my-snippets")]
        [Authorize]
        public async Task<IActionResult> GetMySnippets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? language = null,
            [FromQuery] string? search = null)

        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }
            var filter = new SnippetFilterDto
            {
                Language = language,
                Search = search,
                AuthorId = userId.Value,
                IsPublic = null // Show both public and private for own snippets
            };
            var result = await _snippetService.GetAllFilteredAsync(filter, page, pageSize, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _snippetService.GetByIdAsync(id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSnippetDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                var result = await _snippetService.CreateAsync(createDto, userId.Value);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateSnippetDto updateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                var result = await _snippetService.UpdateAsync(id, updateDto, userId.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                await _snippetService.DeleteAsync(id, userId.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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