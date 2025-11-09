using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.DCC.Option;
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