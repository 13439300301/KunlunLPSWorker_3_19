using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_TopupGfit)]
    public class RechargeAmountGiftCouponMessage : BaseMessage
    {
        //会员卡Id
        public long MembershipCardId { get; set; }

        //本次充值金额
        public decimal TopupAmount { get; set; }

        public string UserCode { get; set; }
    }
}
