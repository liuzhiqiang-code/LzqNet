public class SwaggerOption
{
    public string GatewayUrl { get; set; }
    public List<SwaggerEndpoint> Endpoints { get; set; } = [];
}

public class SwaggerEndpoint
{
    public string Key { get; set; }
    public string Title { get; set; }
    public string Version { get; set; } = "v1";
    public string Url { get; set; }
}