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
        _logger.LogInformation("Input: {Input}", input);
        _logger.LogInformation("Traceparent: {Traceparent}", traceparent);

        var updatedInput = input with { DemoValue = input.DemoValue + " and from Service B" };

        // Example 2 without cloudevent.traceparent header/metadata:
        await _daprClient.PublishEventAsync("rabbitmq-pubsub-service-b", "queue-y", updatedInput);

        // Example 3 with cloudevent.traceparent header/metadata:
        // var metadata = new Dictionary<string, string>
        // {
        //     { "cloudevent.traceparent", traceparent },
        // };
        //
        // await _daprClient.PublishEventAsync("rabbitmq-pubsub-service-b", "queue-y", updatedInput, metadata);
        
        return Ok();
    }
}

public record DemoPayload(string DemoValue, DateTimeOffset DemoTimestamp);