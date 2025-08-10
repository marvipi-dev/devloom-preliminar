using System.Diagnostics;

namespace DevloomPreliminar.ExtensionMethods;

public static class ValidationExtensions
{
    public static HttpValidationProblemDetails ToProblemDetails(this Dictionary<string, string[]> erros, HttpContext context)
    {
        return new(erros)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Extensions = new Dictionary<string, object?>
                { { "traceId", Activity.Current?.Id ?? context?.TraceIdentifier } }
        };
    }
}