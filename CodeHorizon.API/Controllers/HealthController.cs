using Microsoft.AspNetCore.Mvc;

namespace CodeHorizon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Message = "Code Horizon API is running!",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
