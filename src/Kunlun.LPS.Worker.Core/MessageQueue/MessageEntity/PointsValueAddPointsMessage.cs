using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_AddPoints)]
    public class PointsValueAddPointsMessage : PointsValueMessageBase
    {
        public long PointsAccountHistoryId { get; set; }
    }
}
