using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponType : BaseEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public CouponCategory Category { get; set; }

        public CouponTimeLimitMode TimeLimitMode { get; set; }

        public DateTime? TimeLimitBeginDate { get; set; }

        public DateTime? TimeLimitEndDate { get; set; }

        public int? TimeLimitDays { get; set; }

        public string Description { get; set; }

        public string Notice { get; set; }

        public decimal ExchangeNeedPoints { get; set; }

        public int TotalCount { get; set; }

        public int InventoryAlertLine { get; set; }

        public string Prefix { get; set; }

        public decimal? FaceValue { get; set; }

        public CouponExchangeMode ExchangeMode { get; set; }

        public DateTime? ExchangeBeginDate { get; set; }

        public DateTime? ExchangeEndDate { get; set; }

        public string ClassCode { get; set; }

        public decimal? ExternalPoints { get; set; }

        public int Seq { get; set; }

        public string Udf1 { get; set; }

        public string Udf2 { get; set; }

        public string Udf3 { get; set; }

        public string Udf4 { get; set; }

        public string Udf5 { get; set; }

        public string ExchangeHotelCode { get; set; }

        public bool? NeedManageInventory { get; set; }

        /// <summary>
        /// 是否可以转赠
        /// </summary>
        public bool CanGiveAway { get; set; }

        /// <summary>
        /// 起用金额
        /// </summary>
        public decimal? MinimumAmount { get; set; }

        ///// <summary>
        ///// 起始人数
        ///// </summary>
        public int? PeopleFrom { get; set; }

        ///// <summary>
        ///// 结束人数
        ///// </summary>
        public int? PeopleTo { get; set; }

        /// <summary>
        /// 折扣率
        /// </summary>
        public decimal? DiscountRate { get; set; }

        /// <summary>
        /// 折扣代码
        /// </summary>
        public string DiscountCode { get; set; }

        /// <summary>
        /// 支付账号
        /// </summary>
        public string PaymentAccount { get; set; }
    }
}
