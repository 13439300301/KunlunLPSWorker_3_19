using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_CancelUse)]
    public class CouponCancelUseMessage : CouponMessageBase
    {
        public long couponHistoryId { get; set; }
    }
}
