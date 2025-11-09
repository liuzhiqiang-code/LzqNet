using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.DCC.Option;
public class RateLimiterOption
{
    public required string PolicyName { get; set; }
    public int PermitLimit { get; set; }
    public int WindowInSeconds { get; set; }
    public string? QueueProcessingOrder { get; set; }
    public int? QueueLimit { get; set; }
}
