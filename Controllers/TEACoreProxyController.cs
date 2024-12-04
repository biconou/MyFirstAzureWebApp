using System.Text;
using System.Xml.Linq;
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


    [HttpPost("TeaCoreEncryptedPassword/api/password/getpassword")]
    public async Task<IActionResult> ProcessGetPassword(
    [FromQuery] string APCISeed,
    [FromQuery] string ReleaseId,
    [FromQuery] string DatabaseVersion)
    {
        try
        {
            // Validation des paramètres
            if (string.IsNullOrWhiteSpace(APCISeed) ||
                string.IsNullOrWhiteSpace(ReleaseId) ||
                string.IsNullOrWhiteSpace(DatabaseVersion))
            {
                return BadRequest(new { Error = "Tous les paramètres APCISeed, ReleaseId et DatabaseVersion sont requis." });
            }

            // Générer dynamiquement le XML pour le body à transmettre
            XNamespace ns = "http://esw.volvo.com/encryptedpasswordrequest/1_0";
            var xmlDocument = new XDocument(
                new XElement(ns + "EncryptedPasswordRequest",
                    new XAttribute("requestType", "Read"),
                    new XAttribute("schemaVersion", "1.0"),
                    new XElement(ns + "ReleaseId", ReleaseId),
                    new XElement(ns + "DatabaseVersion",
                        new XAttribute("databaseType", "aftermarket"),
                        DatabaseVersion
                    ),
                    new XElement(ns + "APCISeed", APCISeed)
                )
            );

            // Convertir le document XML en chaîne de caractères
            var xmlString = xmlDocument.ToString(SaveOptions.DisableFormatting);

            // Construire le HttpContent avec le XML généré
            var content = new StringContent(xmlString, Encoding.UTF8, "application/xml");

            // Ajouter les en-têtes de la requête entrante
            foreach (var header in Request.Headers)
            {
                if (header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Construire l'URL de l'endpoint distant avec les paramètres de requête
            var endpoint = "TeaCoreEncryptedPassword/api/password/getpassword";

            // Envoyer la requête POST
            var response = await _httpClient.PostAsync(endpoint, content);

            // Lire et retourner la réponse distante
            var finalContent = await response.Content.ReadAsStringAsync();
            return Ok(finalContent);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { Error = "Erreur lors de l'appel à l'API distante.", Details = ex.Message });
        }
    }



    [HttpPost("{**endpoint}")]
    public async Task<IActionResult> ProcessGenericPost(String endpoint)
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
