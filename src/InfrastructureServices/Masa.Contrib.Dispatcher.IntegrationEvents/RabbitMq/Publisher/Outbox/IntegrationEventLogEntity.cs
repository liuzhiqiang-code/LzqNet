using LzqNet.Common.Contracts;
using SqlSugar;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq.Publisher.Outbox;

[Tenant("MsmConnection"), SugarTable("integration_event_log")]
public class IntegrationEventLogEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "event_id")]
    public Guid EventId { get; set; }

    [SugarColumn(ColumnName = "topic", Length = 100)]
    public string Topic { get; set; }

    [SugarColumn(ColumnName = "content", Length = 1000)]
    public string Content { get; set; }

    [SugarColumn(ColumnName = "state")]
    public IntegrationEventStates State { get; set; }

    [SugarColumn(ColumnName = "times_sent")]
    public int TimesSent { get; set; }

    [SugarColumn(ColumnName = "transaction_id")]
    public Guid TransactionId { get; set; }

    //[SugarColumn(ColumnName = "row_version")]
    //public Guid RowVersion { get; set; } = Guid.NewGuid();

}
