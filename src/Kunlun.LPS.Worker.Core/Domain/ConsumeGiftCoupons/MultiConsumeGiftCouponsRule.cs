using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons
{
    public class MultiConsumeGiftCouponsRule : BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public WeekControlEnum WeekControl { get; set; }

        public BirthdayControlEnum BirthdayControl { get; set; }

        public decimal Revenue { get; set; }

        /// <summary>
        /// 消费类型
        /// </summary>
        public string HistoryTypeCode { get; set; }

        public string Description { get; set; }

        public virtual ICollection<MultiConsumeGiftCouponsRuleMemberSourceMap> MultiConsumeGiftCouponsRuleMemberSourceMaps { get; set; }

        public virtual ICollection<MultiConsumeGiftCouponsRulePaymentMap> MultiConsumeGiftCouponsRulePaymentMaps { get; set; }

        public virtual ICollection<MultiConsumeGiftCouponsRulePlaceMap> MultiConsumeGiftCouponsRulePlaceMaps { get; set; }

        public virtual ICollection<MultiConsumeGiftCouponsRuleCouponsDetail> MultiConsumeGiftCouponsRuleCouponsDetails { get; set; }

        public virtual ICollection<MultiConsumeGiftCouponsRuleConsumeTypeMap> MultiConsumeGiftCouponsRuleConsumeTypeMaps { get; set; }
    }
}
