using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class GrowthAccountHistory : BaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long MembershipCardTypeId { get; set; }
        public long MembershipCardAccountId { get; set; }
        public long MembershipCardId { get; set; }
        public string MembershipCardNumber { get; set; }
        public long? TransactionId { get; set; }
        public long? MembershipCardTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public long? HistoryId { get; set; }
        public Guid BatchId { get; set; }
        public string Description { get; set; }
        public GrowthAccrueType AccrueType { get; set; }
        public decimal Values { get; set; }
        public string HotelCode { get; set; }
        public decimal LastBalance { get; set; }
        public decimal ThisBalance { get; set; }
        public bool IsLastCommand { get; set; }

        public virtual Transaction Transaction { get; set; }

    }
}
