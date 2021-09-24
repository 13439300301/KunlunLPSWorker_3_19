using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.DivisionCards
{
    public class DivisionCardFeeHistory : BaseEntity
    {
        public long Id { get; set; }

        public long CardId { get; set; }

        public string CardNumber { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal Amount { get; set; }

        public int OperationType { get; set; }

        public decimal LastBalance { get; set; }

        public decimal ThisBalance { get; set; }

        public decimal LastOverdraftBalance { get; set; }

        public decimal ThisOverdraftBalance { get; set; }

        public int RevenueType { get; set; }

        public string PaymentMode { get; set; }

        public string PaymentCardNumber { get; set; }

        public string PlaceCode { get; set; }

        public string CheckNumber { get; set; }

        public string Description { get; set; }

        public bool IsManual { get; set; }

        public long? TransactionId { get; set; }

        public long? LeaderStoredValueHistoryId { get; set; }

        public string HotelCode { get; set; }

        public string SourceCode { get; set; }

        public long LeaderMembershipCardId { get; set; }

        public bool IsOverdraft { get; set; }
    }
}
