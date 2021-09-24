using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Kunlun.LPS.Worker.Core.
    Domain.RoomCouponRules;

namespace Kunlun.LPS.Worker.Core.Domain.RoomCouponRules
{
   public class RoomCouponsRule: BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public string WeekControl { get; set; }

        public decimal Revenue { get; set; }

        public RoomCouponsRuleCalculationItem CalculationItem { get; set; }

        public RoomCouponsRuleEventItem EventItem { get; set; }

        public string Description { get; set; }

        public virtual ICollection<RoomCouponsRuleChannel> RoomCouponsRuleChannel { get; set; }
        public virtual ICollection<RoomCouponsRuleCouponsDetail> RoomCouponsRuleCouponsDetail { get; set; }
        public virtual ICollection<RoomCouponsRuleMarket> RoomCouponsRuleMarket { get; set; }
        public virtual ICollection<RoomCouponsRuleMemberSource> RoomCouponsRuleMemberSource { get; set; }
        public virtual ICollection<RoomCouponsRulePayment> RoomCouponsRulePayment { get; set; }
        public virtual ICollection<RoomCouponsRuleRate> RoomCouponsRuleRate { get; set; }
    }
}
