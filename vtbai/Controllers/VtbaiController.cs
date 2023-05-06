using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace vtbai.Controllers
{
    [Route("v1/[controller]/[Action]")]
    [ApiController]
    public class VtbaiController : ControllerBase
    {
        private readonly ILogger<VtbaiController> _logger;

        public VtbaiController(ILogger<VtbaiController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public string QA(string q)
        {
            return new GPTCore().QA(q);
        }
    }
}
