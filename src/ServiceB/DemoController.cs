using System.Diagnostics;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServiceB;

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

        var updatedInput = input with { DemoValue = input.DemoValue + " and from Service B" };

        await _daprClient.PublishEventAsync("rabbitmq-pubsub-service-b", "queue-y", updatedInput);

        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);