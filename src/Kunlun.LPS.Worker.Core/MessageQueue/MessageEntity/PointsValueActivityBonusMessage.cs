using System;
using System.Collections.Generic;
using System.Text;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_ActivityBonus)]
    public class PointsValueActivityBonusMessage : PointsValueMessageBase
    {
        public long HistoryId { get; set; }

    }
}
