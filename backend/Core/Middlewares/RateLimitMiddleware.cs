using Microsoft.Extensions.Caching.Memory;
namespace TalentBridge.Core.Middlewares;
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitMiddleware> _logger;

    private static readonly Dictionary<string, RateLimitConfig> EndpointLimits = new()
    {
        { "default", new RateLimitConfig { RequestsPerMinute = 100, BurstAllowed = 15 } }
    };
    public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var endpoint = GetEndpointPattern(context.Request.Path);
        var config = GetRateLimitConfig(endpoint);

        _logger.LogInformation($"Request: {context.Request.Path}, Matched: {endpoint}, Client: {clientId}, Limit: {config.RequestsPerMinute}/min, Burst: {config.BurstAllowed}");

        var isAllowed = CheckRateLimit(clientId, endpoint, config, context);

        if (!isAllowed)
        {
            _logger.LogWarning($"BLOCKED: {context.Request.Path} for client {clientId}");
            await HandleRateLimitExceeded(context, config);
            return;
        }

        await _next(context);
    }

    private bool CheckRateLimit(string clientId, string endpoint, RateLimitConfig config, HttpContext context)
    {
        var now = DateTime.UtcNow;
        var cacheKey = $"rate_limit_{clientId}_{endpoint}";

        var requestLog = _cache.GetOrCreate(cacheKey, _ => new RequestLog());

        requestLog.Requests.RemoveAll(r => now - r > TimeSpan.FromMinutes(1));

        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
        var effectiveLimit = isAuthenticated ?
            (int)(config.RequestsPerMinute * 1.5) : config.RequestsPerMinute;

        var recentRequests = requestLog.Requests.Count(r => now - r < TimeSpan.FromSeconds(10));
        if (recentRequests >= config.BurstAllowed)
        {
            _logger.LogWarning($"Burst limit exceeded for {clientId} on {endpoint}");
            return false;
        }

        if (requestLog.Requests.Count >= effectiveLimit)
        {
            _logger.LogWarning($"Rate limit exceeded for {clientId} on {endpoint}");
            return false;
        }

        requestLog.Requests.Add(now);
        _cache.Set(cacheKey, requestLog, TimeSpan.FromMinutes(2));

        context.Response.Headers.Append("X-RateLimit-Limit", effectiveLimit.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", (effectiveLimit - requestLog.Requests.Count).ToString());
        context.Response.Headers.Append("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());

        return true;
    }

    private string GetClientIdentifier(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"user:{userId}";
        }

        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey))
            return $"api:{apiKey}";

        var ip = GetClientIP(context);
        var userAgent = context.Request.Headers.UserAgent.ToString();
        return $"ip:{ip}:{userAgent.GetHashCode()}";
    }

    private string GetClientIP(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
        {
            var ips = forwarded.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIP))
            return realIP;

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
    private string GetEndpointPattern(PathString path)
    {
        var pathStr = path.Value?.ToLowerInvariant() ?? "";

        _logger.LogDebug($"Matching path: {pathStr}");

        var patterns = EndpointLimits.Keys.Where(k => k != "default").OrderByDescending(k => k.Length);

        foreach (var pattern in patterns)
        {
            if (pathStr.StartsWith(pattern.ToLowerInvariant()))
            {
                _logger.LogDebug($"Matched pattern: {pattern}");
                return pattern;
            }
        }

        _logger.LogDebug("Using default pattern");
        return "default";
    }

    private RateLimitConfig GetRateLimitConfig(string endpoint)
    {
        return EndpointLimits.TryGetValue(endpoint, out var config) ? config : EndpointLimits["default"];
    }

    private async Task HandleRateLimitExceeded(HttpContext context, RateLimitConfig config)
    {
        context.Response.StatusCode = 429;
        context.Response.Headers.Append("Retry-After", "60");
        context.Response.Headers.Append("X-RateLimit-Limit", config.RequestsPerMinute.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", "0");

        var response = new
        {
            error = "Rate limit exceeded",
            message = "Too many requests. Please slow down and try again in a minute.",
            retryAfter = 60
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
