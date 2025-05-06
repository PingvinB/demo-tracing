using System.Text.Json;
using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace ServiceC;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("hello")]
    public ActionResult Hello(CloudEvent<DemoPayload> input, [FromHeader] string traceparent)
    {
        var serializedCloudEvent = JsonSerializer.Serialize(input);
        _logger.LogInformation("CloudEvent: {CloudEvent}", serializedCloudEvent);
        _logger.LogInformation("Traceparent: {Traceparent}", traceparent);
        
        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);