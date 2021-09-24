using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_Exchange)]
    public class CouponExchangeMessage : CouponMessageBase
    {
        public decimal? TotalNeedsPoints { get; set; }

        public decimal? totalMoney { get; set; }

        public List<CouponListDetail> couponListDetails { get; set; }
    }
}
