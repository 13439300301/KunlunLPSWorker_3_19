using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRules:BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public RoomPointsRulesCalculationMode CalculationMode { get; set; }

        public RoomPointsRulesCalculationItem CalculationItem { get; set; }

        public decimal Limit { get; set; }

        public long TargetMembershipTypeAccountId { get; set; }

        public bool IsAvailable { get; set; }

        public ExpireMode ExpireMode { get; set; }

        public int? Expires { get; set; }

        public string Description { get; set; }
        public TransactionMode TransactionMode { get; set; }

        public int? RateCodeCalculationMode { get; set; }

        public bool ShouldCheckGuestFolioName { get; set; }

        public virtual ICollection<RoomPointsRuleMembershipCardLevel> MembershipCardLevel { get; set; }

        public virtual ICollection<RoomPointsRuleChannel> RoomPointsRuleChannel { get; set; }

        public virtual ICollection<RoomPointsRuleHotel> RoomPointsRuleHotel { get; set; }

        public virtual ICollection<RoomPointsRulePayment> RoomPointsRulePayment { get; set; }

        public virtual ICollection<RoomPointsRuleMarket> RoomPointsRuleMarket { get; set; }

        public virtual ICollection<RoomPointsRuleRateTemplate> RoomPointsRuleRateTemplate { get; set; }

        public virtual ICollection<RoomPointsRuleForBucketMembershipCardLevel> MembershipCardLevelForBucket { get; set; }

        public virtual ICollection<RoomPointsRuleBucketMap> RoomPointsRuleBucketMap { get; set; }
        public virtual ICollection<RoomPointsRuleHotelRoomType> RoomPointsRuleHotelRoomType { get; set; }
    }
}
