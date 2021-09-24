using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_DailyCheckInBonus)]
    public class PointsValueDailyCheckInBonusMessage : PointsValueMessageBase
    {
        public long HistoryId { get; set; }
    }
}
