using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons
{
    public class FbConsumeGiftCouponsRuleCouponsDetail : BaseEntity
    {
        public long Id { get; set; }

        public long FbConsumeGiftCouponsRuleId { get; set; }

        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }

        public virtual FbConsumeGiftCouponsRule FbConsumeGiftCouponsRule { get; set; }
    }
}
