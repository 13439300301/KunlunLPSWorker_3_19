using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 业务交易单表（交易是指当前进行卡值或积分操作）
    /// </summary>
    public class Transaction : BaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 该笔交易总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal RealAmount { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 交易类型枚举
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// 该交易所属档案 Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }

        /// <summary>
        /// 交易场所代码
        /// </summary>
        public string PlaceCode { get; set; }

        /// <summary>
        /// 主记录 Id
        /// </summary>
        public long? MainId { get; set; }

        /// <summary>
        /// 交易描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// 交易积分
        /// </summary>
        public decimal Points { get; set; } = 0;

        /// <summary>
        /// 业务单表的自增id，计划用来生成业务单号，目前没有用到
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? TransactionIdentity { get; set; }

        /// <summary>
        /// 管理费
        /// </summary>
        public decimal? Fee { get; set; }

        /// <summary>
        /// 商户Id
        /// </summary>
        public long? MerchantTypeId { get; set; }

        /// <summary>
        /// 外部单号（储值时存渠道订单号，其他的没用到）
        /// </summary>
        public string CheckNumber { get; set; }

        /// <summary>
        /// 判断是否支付重复
        /// </summary>
        public string SequenceNumber { get; set; }

        public string AttachmentPath { get; set; }

        public virtual ICollection<MembershipCardTransaction> MembershipCardTransactions { get; set; }

    }
}
