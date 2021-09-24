using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Consume_New)]
    public class ConsumeNewMessage : BaseMessage
    {

        public long ConsumeHistoryId { get; set; }

        public long MembershipCardId { get; set; }

        public long? TransactionId { get; set; }
    }
}
