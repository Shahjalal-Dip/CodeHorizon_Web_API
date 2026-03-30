using CodeHorizon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;

    public TagsController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagRepository.GetAllAsync();
        return Ok(tags.Select(t => new { t.Id, t.Name }));
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular([FromQuery] int count = 10)
    {
        // This would need to be implemented
        // Returns most used tags
        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        // Auto-complete for tag input
        var tags = await _tagRepository.GetTagsByNamesAsync(new[] { q });
        return Ok(tags);
    }
}