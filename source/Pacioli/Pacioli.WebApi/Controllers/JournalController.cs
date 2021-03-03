using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pacioli.WebApi.Controllers
{
    [Route("api/[controller]"), ApiController, Authorize(Roles = "Accountant")]
    public class JournalController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLedger()
        {
            return Ok();
        }
    }
}
