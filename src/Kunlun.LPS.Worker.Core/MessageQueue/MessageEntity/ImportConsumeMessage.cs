using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Import_Consume)]
    public class ImportConsumeMessage : ImportBaseMessage
    {
        public string ConsumeTypeCode { get; set; }
    }
}
