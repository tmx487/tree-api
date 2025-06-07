using Microsoft.AspNetCore.Mvc;
using TreeAPI.Domain.Exceptions;

namespace TreeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("throw-secure-exception-query")]
        public IActionResult ThrowSecureQueryException([FromQuery] string message = "Secure exception from qeury")
        {
            throw new SecureException(message);
        }

        [HttpGet("throw-secure-exception-body")]
        public IActionResult ThrowSecureBodyException([FromBody] string message = "Secure exception from body")
        {
            throw new SecureException(message);
        }

        [HttpGet("throw-notfound-exception")]
        public IActionResult ThrowNodeNotFoundException()
        {
            throw new NodeNotFoundException("Testing: throw node not found exception");
        }

        [HttpGet("throw-generic-exception")]
        public IActionResult ThrowGenericException()
        {
            throw new InvalidOperationException("Testing: throw generic exception");
        }

        [HttpGet("no-exception")]
        public IActionResult NoException()
        => Ok(new {message = "Successfull operation."});
    }
}
