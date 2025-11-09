using Yarp.Telemetry.Consumption;

namespace LzqNet.ApiGateway.Metrics;

/// <summary>
/// 记录 WebSocket 连接关闭事件的相关信息，监视和了解 WebSocket 的性能和行为
/// </summary>
public sealed class WebSocketsTelemetryConsumer(ILogger<WebSocketsTelemetryConsumer> logger) : IWebSocketsTelemetryConsumer
{
    private readonly ILogger<WebSocketsTelemetryConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void OnWebSocketClosed(DateTime timestamp, DateTime establishedTime, WebSocketCloseReason closeReason, long messagesRead, long messagesWritten)
    {
        _logger.LogInformation($"WebSocket connection closed ({closeReason}) after reading {messagesRead} and writing {messagesWritten} messages over {(timestamp - establishedTime).TotalSeconds:N2} seconds.");
    }
}