using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_SourceTransfer)]
    public class SourcePointsValueTransferMessage : BaseMessage
    {
        public long TransactionId { get; set; }

        public string TransactionNumber { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string MembershipCardNumber { get; set; }

        public long? MembershipCardId { get; set; }

        public decimal? Points { get; set; }
    }
}
