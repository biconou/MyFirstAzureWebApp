using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyFirstAzureWebApp.Controllers
{
    [Route("[controller]/{*path}")]
    [ApiController]
    public class RequestLoggerController : ControllerBase
    {
        private readonly ILogger<RequestLoggerController> _logger;

        public RequestLoggerController(ILogger<RequestLoggerController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [HttpGet]
        [HttpPut]
        [HttpDelete]
        public IActionResult LogRequest()
        {
            // Log request method and path
            _logger.LogInformation("Request received: {Method} {Path}", Request.Method, Request.Path);

            // Log headers
            foreach (var header in Request.Headers)
            {
                _logger.LogInformation("Header: {Key} = {Value}", header.Key, header.Value);
            }

            // Log query parameters (if any)
            if (Request.Query.Any())
            {
                foreach (var queryParam in Request.Query)
                {
                    _logger.LogInformation("Query: {Key} = {Value}", queryParam.Key, queryParam.Value);
                }
            }

            // Log body (for POST/PUT)
            if (Request.ContentLength > 0 && 
                (Request.Method == HttpMethod.Post.Method || Request.Method == HttpMethod.Put.Method))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    _logger.LogInformation("Body: {Body}", body);
                }
            }

            return Ok("Request logged.");
        }
    }
}
