using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_Expired)]
    public class PointsExpireMessage : PointsValueMessageBase
    {
    }
}