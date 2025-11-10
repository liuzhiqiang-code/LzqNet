public class RateLimiterOption
{
    public required string PolicyName { get; set; }
    public int PermitLimit { get; set; }
    public int WindowInSeconds { get; set; }
    public string? QueueProcessingOrder { get; set; }
    public int? QueueLimit { get; set; }
}
