using System.Diagnostics;
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
        _logger.LogInformation("Traceparent: {Traceparent}", traceparent);
        _logger.LogInformation("Current Activity Id: {ActivityId} (ParentSpanId: {ParentSpanId})", Activity.Current?.Id,
            Activity.Current?.ParentSpanId);
        
        var serializedCloudEvent = JsonSerializer.Serialize(input);
        _logger.LogInformation("CloudEvent: {CloudEvent}", serializedCloudEvent);
        
        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);