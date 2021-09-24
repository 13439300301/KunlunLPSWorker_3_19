using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;

namespace Kunlun.LPS.Core.MessageQueue
{
    [RoutingKey(RoutingKeys.StoredValue_CardValueAwardCancel)]
    public class CancelRewardsMessage : BaseMessage
    {
        public long StoredValueAccountHistoryId { get; set; }

        public long? MembershipCardId { get; set; }

        public string MembershipCardNumber { get; set; }

        public decimal? Amount { get; set; }

        public decimal? ThisBalance { get; set; }

        public DateTime? TransactionDate { get; set; }

        public decimal? LastBalance { get; set; }
    }
}
