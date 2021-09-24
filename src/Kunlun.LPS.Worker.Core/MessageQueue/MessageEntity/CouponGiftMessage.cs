using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_Gift)]
    public class CouponGiftMessage : CouponMessageBase
    {
        public List<CouponListDetail> couponListDetails { get; set; }
    }
}
