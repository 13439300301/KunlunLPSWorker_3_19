using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_WorkerBatchGift)]
    public class WorkerBatchGiftCouponMessage : CouponMessageBase
    {
        public List<long> ProfileIdList { get; set; }

        public List<ExchangeCouponTypeModel> ExchangeCoupons { get; set; }

        public List<ExchangeCouponPackage> ExchangeCouponPackages { get; set; }

        public long CouponChannel { get; set; }

        public string UserCode { get; set; }

    }
}
