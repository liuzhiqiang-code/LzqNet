using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

namespace LzqNet.Template.Contracts.Events;

public record TemplateEvent : IntegrationEvent
{
    public override string Topic { get; set; } = "template.message";

    /// <summary>
    /// 
    /// </summary>
    public List<long> TemplateEventProp { get; set; }
}