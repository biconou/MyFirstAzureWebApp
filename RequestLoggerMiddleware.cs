using System.Diagnostics;
using System.Text;
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

        context.Request.EnableBuffering();

        // Capture des headers et autres informations
        var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        var queryParams = context.Request.QueryString.Value;
        var body = "";
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            // Réinitialiser la position pour permettre une lecture ultérieure
            context.Request.Body.Position = 0;
        }


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

        context.Request.Body.Position = 0;

        // Passer la requête au middleware suivant
        await _next(context);
    }
}
