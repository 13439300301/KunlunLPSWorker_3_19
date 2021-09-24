using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_Change)]
    public class CouponChangeMessage:BaseMessage
    {
        public long ProfileId { get; set; }
        public string ProviderKey { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string OperationType { get; set; }
        public string CouponName { get; set; }
        public string CouponNumber { get; set; }
        public DateTime TransactionTime { get; set; }
        public string HotelName { get; set; }
        public string PlaceName { get; set; }
        public string CouponStatus { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
