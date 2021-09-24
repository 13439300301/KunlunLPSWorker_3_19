using Kunlun.LPS.Worker.Core.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRulePayment:BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleId { get; set; }

        public string PaymentCode { get; set; }

        public virtual RoomPointsRules RoomPointsRule { get; set; }
    }
}
