using System.ComponentModel;

namespace LzqNet.Test.Domain.Consts;

/// <summary>
/// AI调用工具函数服务
/// </summary>
public class AIFunctionTools
{
    [Description("Get the weather for a given location.")]
    public static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
}
