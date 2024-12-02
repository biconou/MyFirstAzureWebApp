using Microsoft.AspNetCore.Mvc;

namespace MyFirstAzureWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TEACoreProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TEACoreProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ProxyApi");
    }

    private void LogRequestDetails(string endpoint)
    {
        var request = HttpContext.Request;
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<TEACoreProxyController>>();

        // Log des informations de base
        logger.LogInformation("Requête entrante pour le proxy :");
        logger.LogInformation("URL : {Url}", request.Path);
        logger.LogInformation("Méthode : {Method}", request.Method);
        logger.LogInformation("Endpoint cible : {Endpoint}", endpoint);

        // Log des paramètres de requête
        if (request.Query.Any())
        {
            logger.LogInformation("Paramètres de requête :");
            foreach (var queryParam in request.Query)
            {
                logger.LogInformation("{Key} = {Value}", queryParam.Key, queryParam.Value);
            }
        }

        // Log des en-têtes
        logger.LogInformation("En-têtes HTTP :");
        foreach (var header in request.Headers)
        {
            logger.LogInformation("{Key}: {Value}", header.Key, header.Value);
        }

        // Log du payload si disponible
        if (request.ContentLength > 0 && (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put))
        {
            request.Body.Position = 0; // Réinitialiser le stream
            using var reader = new StreamReader(request.Body);
            var bodyContent = reader.ReadToEndAsync().Result;
            logger.LogInformation("Corps de la requête : {Body}", bodyContent);
        }
    }


    // Proxy d'un GET vers l'API externe
    [HttpGet("{endpoint}")]
    public async Task<IActionResult> GetProxy(string endpoint)
    {
        try
        {
            // Log des informations de la requête entrante
            LogRequestDetails(endpoint);
            
            // Appeler l'API distante
            var response = await _httpClient.GetAsync(endpoint);

            // Transférer la réponse telle quelle
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (HttpRequestException ex)
        {
            // Gestion des erreurs de connexion
            return StatusCode(500, new { Error = "Une erreur s'est produite lors de l'appel à l'API distante.", Details = ex.Message });
        }
    }
}
