using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace vtbai.Controllers
{
    public class VtbaiController : BaseApiController
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
