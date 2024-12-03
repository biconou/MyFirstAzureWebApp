using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class CustomTextFormatter : TextOutputFormatter
{
    public CustomTextFormatter()
    {
        // Ajouter "text/plain" comme type MIME supporté
        SupportedMediaTypes.Add("text/plain");

        // Ajouter UTF-8 comme encodage supporté
        SupportedEncodings.Add(Encoding.UTF8);
    }

    // Cette méthode vérifie si le type de contenu peut être écrit par ce formatteur
    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return context.Object is string; // Ce formatteur peut écrire des objets de type string
    }

    // Cette méthode effectue l'écriture du contenu dans la réponse HTTP
    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var writer = context.HttpContext.Response.Body;
        var content = context.Object as string;

        // Convertir le contenu en bytes et l'écrire dans le corps de la réponse
        var bytes = selectedEncoding.GetBytes(content);
        return writer.WriteAsync(bytes, 0, bytes.Length);
    }
}
