using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http.Extensions;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TelemetryClient _telemetryClient;

    public RequestLoggerMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
    {
        _next = next;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture des headers et autres informations
        var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var queryParams = context.Request.QueryString.Value;

        // Créer un événement personnalisé dans Application Insights
        var requestTelemetry = new RequestTelemetry
        {
            Name = context.Request.Path,
            Url = context.Request.GetUri(),
            Timestamp = DateTimeOffset.UtcNow,
            Duration = TimeSpan.Zero,  // La durée sera capturée lors de l'exécution de la requête
            ResponseCode = "200", // Code de statut à définir dynamiquement
        };

        requestTelemetry.Properties["Headers"] = string.Join(", ", headers.Select(kv => $"{kv.Key}: {kv.Value}"));
        requestTelemetry.Properties["Body"] = body;
        requestTelemetry.Properties["QueryParams"] = queryParams;
        requestTelemetry.Properties["usage"] = "debugHttpDetails";

        // Envoyer l'événement personnalisé
        _telemetryClient.TrackRequest(requestTelemetry);

        // Passer la requête au middleware suivant
        await _next(context);
    }
}
