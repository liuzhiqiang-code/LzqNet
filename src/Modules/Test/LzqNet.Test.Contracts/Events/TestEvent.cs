using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

namespace LzqNet.Test.Contracts.Events;

public record TestEvent : IntegrationEvent
{
    public override string Topic { get; set; } = "test.message";

    /// <summary>
    /// 
    /// </summary>
    public List<long> TestEventProp { get; set; }
}