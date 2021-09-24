using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class RechargeActivityRuleCouponsDetail : BaseEntity
    {
        public long Id { get; set; }

        public long RechargeActivityRuleId { get; set; }

        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }

        public decimal StartAmount { get; set; }

        public decimal EndAmount { get; set; }
    }
}
