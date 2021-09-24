using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_Use)]
    public class CouponUseMessage : CouponMessageBase
    {
        public long? CouponId { get; set; }

        public string CouponCode { get; set; }
    }
}
