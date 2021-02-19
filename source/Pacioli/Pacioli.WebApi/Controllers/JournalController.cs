using Microsoft.AspNetCore.Mvc;

namespace Pacioli.WebApi.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class JournalController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLedger()
        {
            return Unauthorized();
        }
    }
}
