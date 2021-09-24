using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.PointsValue_Change)]
    public class PointsValueChangeMessage:BaseMessage
    {
        public long ProfileId { get; set; }

        public long MembershipCardId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string FullName { get; set; }

        public DateTime? TransactionTime { get; set; }

        public string TransactionNumber { get; set; }

        public TransactionType TransactionType { get; set; }

        public decimal? Points { get; set; }

        public decimal? Balance { get; set; }

        public string Hotel { get; set; }

        public string Place { get; set; }

        public string Description { get; set; }
        public string ProviderKey { get; set; }
    }
}
