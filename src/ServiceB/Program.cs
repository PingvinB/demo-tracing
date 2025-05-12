using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDaprClient();

// Used when manually starting activities
builder.Services.AddSingleton<Instrumentation>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(serviceName: "service-b"))
    .WithTracing(c =>
    {
        // Grpc.Net.Client is built on top of HttpClient. When instrumentation for both
        // libraries is enabled, `SuppressDownstreamInstrumentation` prevents the
        // HttpClient instrumentation from generating an additional activity. Additionally,
        // since HttpClient instrumentation is normally responsible for propagating context
        // (ActivityContext and Baggage), Grpc.Net.Client instrumentation propagates
        // context when `SuppressDownstreamInstrumentation` is enabled.
        const bool suppressDownstreamInstrumentation = true;

        c.AddSource("service-b")
            .AddAspNetCoreInstrumentation(cfg =>
            {
                cfg.Filter = context =>
                {
                    string[] excludePrefixes = ["/dapr/subscribe", "/dapr/config"];
                    // Filter out health check requests
                    var shouldInclude =
                        !excludePrefixes.Any(prefix => context.Request.Path.StartsWithSegments(prefix));

                    return shouldInclude;
                };
            })
            .AddGrpcClientInstrumentation(cfg =>
            {
                cfg.SuppressDownstreamInstrumentation = suppressDownstreamInstrumentation;
            })
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri("http://jaeger:4317");
                opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            })
            .SetSampler(new AlwaysOnSampler());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapSubscribeHandler();

await app.RunAsync();