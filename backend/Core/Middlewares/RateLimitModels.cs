namespace TalentBridge.Core.Middlewares;
public class RateLimitConfig
{
    public int RequestsPerMinute { get; set; }
    public int BurstAllowed { get; set; }
}

public class RequestLog
{
    public List<DateTime> Requests { get; set; } = new();
}