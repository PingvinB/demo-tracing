using System.Diagnostics;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;
    private readonly DaprClient _daprClient;

    public DemoController(ILogger<DemoController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }
    
    [HttpPost("hello")]
    public async Task<ActionResult> Hello(DemoPayload input, [FromHeader] string traceparent)
    {
        _logger.LogInformation("Traceparent: {Traceparent}", traceparent);
        _logger.LogInformation("Current Activity Id: {ActivityId} (ParentSpanId: {ParentSpanId})", Activity.Current?.Id,
            Activity.Current?.ParentSpanId);
        
        var updatedInput = input with { DemoValue = "Hello from Service A" };
        
        var request = _daprClient.CreateInvokeMethodRequest(
            "service-b",
            "api/demo/hello",
            updatedInput);
        
        request.Headers.Add("traceparent", Activity.Current?.Id ?? traceparent);

        await _daprClient.InvokeMethodAsync(request);
        
        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);