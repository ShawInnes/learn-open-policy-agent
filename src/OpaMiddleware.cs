using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpaDemo;

public class AuthorizationResult
{
    [JsonPropertyName("result")]
    public bool IsAuthorized { get; set; }
    [JsonPropertyName("decision_id")]
    public string DecisionId { get; set; }
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}

public class OpaAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _opaUrl;
    private readonly HttpClient _httpClient = new();

    public OpaAuthorizationMiddleware(RequestDelegate next, string opaUrl)
    {
        _next = next;
        _opaUrl = opaUrl;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Evaluate the policy using OPA
        var input = new
        {
            input = new
            {
                method = context.Request.Method,
                path = context.Request.Path.Value?.Split("/", StringSplitOptions.RemoveEmptyEntries),
                user = context.User?.Identity?.Name ?? "anonymous",
            }
        };

        var jsonContent = JsonSerializer.Serialize(input);
        var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_opaUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthorizationResult>(responseBody);

        // Check if the request is authorized
        if (result.IsAuthorized)
        {
            await _next(context);
        }
        else
        {
            context.Response.Headers["X-Reason"] = result.Reason;
            context.Response.Headers["X-Decision-Id"] = result.DecisionId;
            context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
        }
    }
}