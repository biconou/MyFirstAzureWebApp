using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MyFirstAzureWebApp.Controllers
{
    [ApiController]
    public class TeaCoreEncryptedPasswordMockController : ControllerBase
    {
        [HttpPost("TeaCoreEncryptedPassword/api/password/getpassword")]
        public IActionResult GetPassword()
        {
            // Réponse fixe
            string response = "You invoked mock TeaCoreEncryptedPassword/api/password/getpassword";

            // Retourner la réponse avec le type MIME "text/plain"
            return Content(response, "text/plain", Encoding.UTF8);
        }

        [HttpPost("TeaCoreApciPatcher/api/teacoreapcidataupdate/getpatch")]
        public IActionResult GetPatch()
        {
            // Réponse fixe
            string response = "You invoked mock TeaCoreApciPatcher/api/teacoreapcidataupdate/getpatch";

            // Retourner la réponse avec le type MIME "text/plain"
            return Content(response, "text/plain", Encoding.UTF8);
        }

        [HttpPost("TeaCoreApciPatcher/api/teacoreapcidataupdate/calculatekey")]
        public IActionResult CalculateKey()
        {
            // Réponse fixe
            string response = "You invoked mock TeaCoreApciPatcher/api/teacoreapcidataupdate/calculatekey";

            // Retourner la réponse avec le type MIME "text/plain"
            return Content(response, "text/plain", Encoding.UTF8);
        }
    }
}
