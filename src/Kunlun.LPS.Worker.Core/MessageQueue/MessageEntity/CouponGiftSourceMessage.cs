using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_GiftSource)]
    public class CouponGiftSourceMessage : BaseMessage
    {
        public List<CouponListDetail> couponListDetails { get; set; }
    }
}
