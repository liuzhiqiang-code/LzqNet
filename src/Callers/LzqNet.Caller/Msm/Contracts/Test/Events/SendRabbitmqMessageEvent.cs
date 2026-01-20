using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Msm.Contracts.Test.Events;

public record SendRabbitmqMessageEvent : IntegrationEvent
{
    public override string Topic { get; set; } = nameof(SendRabbitmqMessageEvent);//topic name

    public string Message { get; set; }
}
