using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule
{
    public class CustomDateCouponRulesCouponsDetail : BaseEntity
    {
        public long Id { get; set; }

        public long CustomDateCouponRulesId { get; set; }

        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }

        public virtual CustomDateCouponRules CustomDateCouponRules { get; set; }
    }
}
