using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ConsumeHistory : BaseEntity
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        /// <summary>
        /// 每条交易的识别信息
        /// </summary>
        public string TraceId { get; set; }

        public long MembershipCardId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string CheckNumber { get; set; }

        public string ConsumeTypeCode { get; set; }

        public DateTime TransactionTime { get; set; }

        public string StoreCode { get; set; }

        public string OutletCode { get; set; }

        /// <summary>
        /// 总消费金额，最终需要支付的金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 不包含税费，服务费等的总消费，主要是消费品的消费金额
        /// </summary>
        public decimal SubTotalAmount { get; set; }

        public decimal Tax { get; set; }

        public decimal SurCharges { get; set; }

        public string CurrencyCode { get; set; }

        /// <summary>
        /// 客人数
        /// </summary>
        public int Guests { get; set; }

        /// <summary>
        /// 是否冲账
        /// </summary>
        public bool IsVoid { get; set; }

        public string OperatorCode { get; set; }

        public string ExternalOperatorId { get; set; }

        public string Description { get; set; }

        public string GuestRemark { get; set; }

        public string InternalRemark { get; set; }

        public string ReservationNumber { get; set; }

        public string TerminalId { get; set; }


        public decimal Points { get; set; }

        public bool IsPointsCalculated { get; set; }

        public decimal Mileages { get; set; }

        public bool IsMileagesCalculated { get; set; }

        public decimal Growth { get; set; }

        public bool IsGrowthCalculated { get; set; }

        public string RM_SourceCode { get; set; }

        public DateTime? RM_ArrivalTime { get; set; }

        public DateTime? RM_DepartureTime { get; set; }

        public string RM_RoomTypeCode { get; set; }

        public string RM_RoomNumber { get; set; }

        public string RM_RoomRateCode { get; set; }

        public decimal? RM_RoomRate { get; set; }

        public int? RM_Nights { get; set; }

        public string RM_MarketCode { get; set; }

        public string RM_ChannelCode { get; set; }

        public long? FB_MealPeriodId { get; set; }

        public DateTime? FB_CheckOpenTime { get; set; }

        public string FB_OrderTypeCode { get; set; }

        public string FB_TableNumber { get; set; }

        public long? TransactionId { get; set; }

        public string ExtenalSystemName { get; set; }

        public decimal? RM_RoomRevenue { get; set; }

        public decimal? RM_FbRevenue { get; set; }

        public decimal? RM_OtherRevenue { get; set; }

        public string RM_PaymentCode { get; set; }

        public string RM_BookerMembershipCardNumber { get; set; }

        public long? RM_BookerMembershipCardId { get; set; }

        public long? ProfileExternalMembershipId { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }

        public bool IsManual { get; set; }

        public bool IsComplete { get; set; }

        public long? MainMembershipCardId { get; set; }

        public bool IsConsumeGiftCouponCalculated { get; set; } = false;

        public int? ImportId { get; set; }

        public string FFPNumber { get; set; }

        public string FFPCode { get; set; }
    }
}
