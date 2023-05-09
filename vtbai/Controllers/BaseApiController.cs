using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace vtbai.Controllers
{
    [Route("v1/[controller]/[Action]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
