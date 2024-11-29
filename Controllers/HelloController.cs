using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        // Endpoint GET : /hello
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = "Bonjour, Azure!" });
        }

        // Endpoint GET avec un paramètre : /hello/{name}
        [HttpGet("{name}")]
        public IActionResult GetWithName(string name)
        {
            return Ok(new { Message = $"Bonjour, {name}!" });
        }
    }
}
