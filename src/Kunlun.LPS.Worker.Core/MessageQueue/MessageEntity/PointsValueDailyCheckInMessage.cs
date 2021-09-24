using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_DailyCheckIn)]
    public class PointsValueDailyCheckInMessage : PointsValueMessageBase
    {
        public long HistoryId { get; set; }
    }
}
