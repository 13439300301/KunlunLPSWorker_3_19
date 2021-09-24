using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.DivisionCards
{
    public class DivisionCardRequest
    {
        public long LeaderCardId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public int OperationType { get; set; }

        public int RevenueType { get; set; }

        public string PaymentMode { get; set; }

        public string PaymentCardNumber { get; set; }

        public string PlaceCode { get; set; }

        public string CheckNumber { get; set; }

        public string Description { get; set; }

        public bool IsManual { get; set; }

        public long? TransactionId { get; set; }

        public long? CardTransactionId { get; set; }

        public long? StoredValueHistoryId { get; set; }

        public string HotelCode { get; set; }

        public string SourceCode { get; set; }

        public decimal OverdraftAmount { get; set; }
    }
}
