using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MyFirstAzureWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TEACoreProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TEACoreProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TEACoreProxy");
    }


    [HttpPost("{**endpoint}")]
    public async Task<IActionResult> ProcessPost(String endpoint)
    {
        try
        {
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            // Copier les headers de la requête entrante
            var requestHeaders = Request.Headers;

            // Construire le HttpContent pour la requête sortante
            var content = new StringContent(body, Encoding.UTF8, Request.ContentType ?? "application/json");

            // Ajouter les headers nécessaires à la requête sortante
            foreach (var header in requestHeaders)
            {
                if (!content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Envoyer la requête POST
            var response = await _httpClient.PostAsync(endpoint, content);

            var finalContent = await response.Content.ReadAsStringAsync();

            // Retourner la réponse distante
            return StatusCode((int)response.StatusCode, finalContent);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { Error = "Erreur lors de l'appel à l'API distante.", Details = ex.Message });
        }
    }
}
