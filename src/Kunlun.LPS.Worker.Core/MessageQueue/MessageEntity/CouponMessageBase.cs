using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class CouponMessageBase:BaseMessage
    {
        public long TransactionId { get; set; }

        public long ProfileId { get; set; }

        public string HotelCode { get; set; }

        public string PlaceCode { get; set; }
    }
}
