using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.FTPStoredValueAccountHistory
{
    public class FTPStoredValueAccountHistoryInfo
    {

        public string MembershipCardTypeCode { get; set; }
        public string Date { get; set; }

        public string HotelCode { get; set; }

        public string MemberNo { get; set; }

        public decimal StoredValueAmount { get; set; }

        public decimal ConsumptionAmount { get; set; }

        public decimal RefundAmount { get; set; }

        public decimal DebitBalanceAmount { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

    }
}
