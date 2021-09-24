using Kunlun.LPS.Worker.Core.Domain.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class Coupon : BaseVersionedEntity
    {
        public long Id { get; set; }

        public long CouponTypeId { get; set; }

        public CouponCategory CouponCategory { get; set; }

        public string Number { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? BindingDate { get; set; }

        public DateTime? UseDate { get; set; }

        public long? OwnerId { get; set; }

        public long? UserId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsExpired { get; set; }

        public long? CouponChannelId { get; set; }

        public string PlaceCode { get; set; }

        public decimal Points { get; set; }

        public decimal? FaceValue { get; set; }

        public decimal? DiscountRate { get; set; }

        public CouponExchangeMode? ExchangeMode { get; set; }

        public long? ExchangeFromId { get; set; }

        public decimal? UnitPrice { get; set; }

        public string CheckNumber { get; set; }
        public bool IsActivation { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberPrefix { get; set; }
    }
}
