using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_CardValueAwardTopup)]
    public class CardValueAwardTopupMessage:StoredValueMessageBase
    {
        public long StoredValueAccountHistoryId { get; set; }

        public string MembershipCardNumber { get; set; }

        public decimal? Amount { get; set; }

        public decimal? ThisBalance { get; set; }

        public DateTime? TransactionDate { get; set; }

        public decimal? LastBalance { get; set; }
    }
}
