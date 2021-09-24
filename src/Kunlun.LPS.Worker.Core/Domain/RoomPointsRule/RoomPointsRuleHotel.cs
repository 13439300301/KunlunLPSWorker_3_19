using Kunlun.LPS.Worker.Core.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRuleHotel:BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleId { get; set; }

        public string HotelCode { get; set; }

        public virtual RoomPointsRules RoomPointsRule { get; set; }
    }
}
