using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ServiceB;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    [HttpPost("hello")]
    public ActionResult Hello(DemoPayload input)
    {
        Debug.WriteLine(input);
        
        return Ok();
    }
}

public record DemoPayload(string Value, DateTimeOffset Timestamp);