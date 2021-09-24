using Kunlun.LPS.Worker.Core.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRuleChannel:BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleId { get; set; }

        public string ChannelCode { get; set; }

        public virtual RoomPointsRules RoomPointsRule { get; set; }
    }
}
