using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ConsumeHistoryDetail : BaseEntity
    {
        public long Id { get; set; }

        public long HistoryId { get; set; }

        public string ItemType { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string MembershipCardNumber { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; }

        public decimal Tax { get; set; }

        public DateTime PostTime { get; set; }

        public int? Seq { get; set; }

        public string Description { get; set; }

        public string RateCode { get; set; }

        //public virtual ConsumeHistory ConsumeHistory { get; set; }

        public string RoomNo { get; set; }
    }
}
