using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Multi_ConsumeGiftCoupons)]
    public class MultiConsumeGiftCouponsMessage : BaseMessage
    {
        public long ConsumeHistoryId { get; set; }

        public string MemberSourceCode { get; set; }

        public string PaymentCode { get; set; }

        public string PlaceCode { get; set; }

        public string ConsumeTypeCode { get; set; }
        public string UserCode { get; set; }
    }
}
