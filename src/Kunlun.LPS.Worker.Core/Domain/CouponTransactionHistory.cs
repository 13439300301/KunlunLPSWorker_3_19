using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponTransactionHistory : BaseEntity
    {
        public long Id { get; set; }

        public long? TransactionId { get; set; }

        public long? MembershipCardTransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public long? HistoryId { get; set; }

        public Guid BatchId { get; set; }

        public string Description { get; set; }

        public long CouponId { get; set; }

        public long CouponTypeId { get; set; }

        public decimal Points { get; set; }

        public string OperatorId { get; set; }

        public long? ProfileId { get; set; }

        public CouponTransactionHistoryOperationType OperationType { get; set; }

        public string CheckNumber { get; set; }

        public string PlaceCode { get; set; }

        public string HotelCode { get; set; }

        public decimal? FaceValue { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
