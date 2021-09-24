using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_PaymentCancel)]
    public class CancelPointsValuePaymentMessage : PointsValueMessageBase
    {
        public long? OldTransactionId { get; set; }

        public string MembershipCardNumber { get; set; }
    }
}
