using Microsoft.AspNetCore.Mvc;

namespace OSApiInterface.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // GET
        public ActionResult<string> Index()
        {
            return "echo";
        }
    }
}