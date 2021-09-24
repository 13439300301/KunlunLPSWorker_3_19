using Kunlun.LPS.Worker.Core.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRuleMembershipCardLevel:BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public decimal Revenue { get; set; }

        public decimal Points { get; set; }

        public decimal RoomRevenue { get; set; }

        public decimal RoomPoints { get; set; }

        public decimal FbRevenue { get; set; }

        public decimal FbPoints { get; set; }

        public decimal OtherRevenue { get; set; }

        public decimal OtherPoints { get; set; }

        public decimal Tax { get; set; }
        public decimal TaxPoints { get; set; }

        public virtual RoomPointsRules RoomPointsRule { get; set; }


    }
}
