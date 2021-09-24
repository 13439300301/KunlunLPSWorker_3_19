using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Fb_ConsumeGiftCoupons)]
    public class FbConsumeGiftCouponsMessage : BaseMessage
    {
        public long ConsumeHistoryId { get; set; }

        public string MemberSourceCode { get; set; }

        public string PaymentCode { get; set; }

        public string PlaceCode { get; set; }
        public string UserCode { get; set; }
    }
}
