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
        _logger.LogInformation("Input: {Input}", input);
        _logger.LogInformation("Traceparent: {Traceparent}", traceparent);
        
        var updatedInput = input with { DemoValue = "Hello from Service A" };
        
        var request = _daprClient.CreateInvokeMethodRequest(
            "service-b",
            "api/demo/hello",
            updatedInput);
        
        request.Headers.Add("traceparent", traceparent);

        await _daprClient.InvokeMethodAsync(request);
        
        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);