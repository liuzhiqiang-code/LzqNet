using System.Text.Json.Serialization;

namespace LzqNet.Caller.ApiGateway.Contracts;

public class ProxyConfigModel
{
    [JsonPropertyName("routes")]
    public List<RouteConfigModel> Routes { get; set; }

    [JsonPropertyName("clusters")]
    public List<ClusterConfigModel> Clusters { get; set; }
}

public class RouteConfigModel
{
    [JsonPropertyName("routeId")]
    public string RouteId { get; set; }

    [JsonPropertyName("match")]
    public RouteMatch Match { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }

    [JsonPropertyName("clusterId")]
    public string ClusterId { get; set; }

    //[JsonPropertyName("authorizationPolicy")]
    //public string AuthorizationPolicy { get; set; }

    [JsonPropertyName("rateLimiterPolicy")]
    public string RateLimiterPolicy { get; set; }

    //[JsonPropertyName("outputCachePolicy")]
    //public string OutputCachePolicy { get; set; }

    //[JsonPropertyName("timeoutPolicy")]
    //public string TimeoutPolicy { get; set; }

    //[JsonPropertyName("timeout")]
    //public string Timeout { get; set; }

    //[JsonPropertyName("corsPolicy")]
    //public string CorsPolicy { get; set; }

    //[JsonPropertyName("maxRequestBodySize")]
    //public long? MaxRequestBodySize { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; }

    [JsonPropertyName("transforms")]
    public List<Dictionary<string, string>> Transforms { get; set; }
}

public class RouteMatch
{
    [JsonPropertyName("methods")]
    public List<string> Methods { get; set; }

    [JsonPropertyName("hosts")]
    public List<string> Hosts { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("queryParameters")]
    public List<QueryParameterMatch> QueryParameters { get; set; }

    [JsonPropertyName("headers")]
    public List<HeaderMatch> Headers { get; set; }
}

public class QueryParameterMatch
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("values")]
    public List<string> Values { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("isCaseSensitive")]
    public bool IsCaseSensitive { get; set; }
}

public class HeaderMatch
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("values")]
    public List<string> Values { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("isCaseSensitive")]
    public bool IsCaseSensitive { get; set; }
}

public class ClusterConfigModel
{
    [JsonPropertyName("clusterId")]
    public string ClusterId { get; set; }

    //[JsonPropertyName("loadBalancingPolicy")]
    //public string LoadBalancingPolicy { get; set; }

    //[JsonPropertyName("sessionAffinity")]
    //public SessionAffinityConfig SessionAffinity { get; set; }

    [JsonPropertyName("healthCheck")]
    public HealthCheckConfig HealthCheck { get; set; }

    //[JsonPropertyName("httpClient")]
    //public HttpClientConfig HttpClient { get; set; }

    //[JsonPropertyName("httpRequest")]
    //public ForwarderRequestConfig HttpRequest { get; set; }

    [JsonPropertyName("destinations")]
    public Dictionary<string, DestinationConfigModel> Destinations { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; }
}

public class SessionAffinityConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("policy")]
    public string Policy { get; set; }

    [JsonPropertyName("failurePolicy")]
    public string FailurePolicy { get; set; }

    [JsonPropertyName("affinityKeyName")]
    public string AffinityKeyName { get; set; }

    [JsonPropertyName("cookie")]
    public SessionAffinityCookieConfig Cookie { get; set; }
}

public class SessionAffinityCookieConfig
{
    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("sameSite")]
    public string SameSite { get; set; }

    [JsonPropertyName("httpOnly")]
    public bool HttpOnly { get; set; }

    [JsonPropertyName("maxAge")]
    public string MaxAge { get; set; }

    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    [JsonPropertyName("isEssential")]
    public bool IsEssential { get; set; }

    [JsonPropertyName("securePolicy")]
    public string SecurePolicy { get; set; }
}

public class HealthCheckConfig
{
    [JsonPropertyName("passive")]
    public PassiveHealthCheckConfig Passive { get; set; }

    [JsonPropertyName("active")]
    public ActiveHealthCheckConfig Active { get; set; }

    [JsonPropertyName("availableDestinationsPolicy")]
    public string AvailableDestinationsPolicy { get; set; }
}

public class PassiveHealthCheckConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("policy")]
    public string Policy { get; set; }

    [JsonPropertyName("reactivationPeriod")]
    public string ReactivationPeriod { get; set; }
}

public class ActiveHealthCheckConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("interval")]
    public string Interval { get; set; }

    [JsonPropertyName("timeout")]
    public string Timeout { get; set; }

    [JsonPropertyName("policy")]
    public string Policy { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("query")]
    public string Query { get; set; }
}

public class HttpClientConfig
{
    [JsonPropertyName("sslProtocols")]
    public System.Security.Authentication.SslProtocols? SslProtocols { get; set; }

    [JsonPropertyName("dangerousAcceptAnyServerCertificate")]
    public bool DangerousAcceptAnyServerCertificate { get; set; }

    [JsonPropertyName("maxConnectionsPerServer")]
    public int? MaxConnectionsPerServer { get; set; }

    [JsonPropertyName("enableMultipleHttp2Connections")]
    public bool EnableMultipleHttp2Connections { get; set; }

    [JsonPropertyName("requestHeaderEncoding")]
    public string RequestHeaderEncoding { get; set; }

    [JsonPropertyName("responseHeaderEncoding")]
    public string ResponseHeaderEncoding { get; set; }

    [JsonPropertyName("webProxy")]
    public WebProxyConfig WebProxy { get; set; }
}

public class WebProxyConfig
{
    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("bypassOnLocal")]
    public bool BypassOnLocal { get; set; }

    [JsonPropertyName("useDefaultCredentials")]
    public bool UseDefaultCredentials { get; set; }
}

public class ForwarderRequestConfig
{
    [JsonPropertyName("activityTimeout")]
    public string ActivityTimeout { get; set; }

    [JsonPropertyName("version")]
    public Version Version { get; set; }

    [JsonPropertyName("versionPolicy")]
    public string VersionPolicy { get; set; }

    [JsonPropertyName("allowResponseBuffering")]
    public bool AllowResponseBuffering { get; set; }
}

public class DestinationConfigModel
{
    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("health")]
    public string Health { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; }
}