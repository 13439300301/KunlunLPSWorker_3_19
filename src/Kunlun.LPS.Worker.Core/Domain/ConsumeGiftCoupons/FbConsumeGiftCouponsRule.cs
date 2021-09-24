using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons
{
    public class FbConsumeGiftCouponsRule : BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public WeekControlEnum WeekControl { get; set; }

        public BirthdayControlEnum BirthdayControl { get; set; }

        public decimal Revenue { get; set; }

        public CalculationItem CalculationItem { get; set; }

        public string Description { get; set; }

        public virtual ICollection<FbConsumeGiftCouponsRuleMemberSourceMap> FbconsumeGiftCouponsRuleMembersourceMaps { get; set; }

        public virtual ICollection<FbConsumeGiftCouponsRulePaymentMap> FbConsumeGiftCouponsRulePaymentMaps { get; set; }

        public virtual ICollection<FbConsumeGiftCouponsRulePlaceMap> FbConsumeGiftCouponsRulePlaceMaps { get; set; }

        public virtual ICollection<FbConsumeGiftCouponsRuleCouponsDetail> FbConsumeGiftCouponsRuleCouponsDetails { get; set; }
    }
}
