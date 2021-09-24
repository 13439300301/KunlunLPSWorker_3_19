using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class StoredValueAccountHistory : BaseEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// 对应储值账户的 Id
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// 该卡的卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 该账户对应的账户类型 Id
        /// </summary>
        public long MembershipCardAccountId { get; set; }

        /// <summary>
        /// 卡Id（实际扣卡值的卡 Id）
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 档案Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 该账户所属的会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 卡值操作类型枚举（储值0、退费1、预授权2、结账3、赠送4、转账5、冲账6、调整7、取消赠送8、过期9、合并会员卡10、还款11、初始化透支额度12、透支额度调整13，分裂卡14）
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// 上次余额
        /// </summary>
        public decimal LastBalance { get; set; }

        /// <summary>
        /// 本次余额
        /// </summary>
        public decimal ThisBalance { get; set; }

        /// <summary>
        /// 这笔卡值的过期时间，如果为`null`说明永不过期
        /// </summary>
        public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// 对应消费历史 GUID，（此字段无用）
        /// </summary>
        public long? HistoryId { get; set; }

        /// <summary>
        ///  消费类型枚举（客房 R、餐饮 FB、其他 Other）（注 Revenue 翻译过来是收入）
        /// </summary>
        public int RevenueType { get; set; }

        /// <summary>
        /// 支付方式（与dic_payment关联）
        /// </summary>
        public string PaymentMode { get; set; }

        /// <summary>
        /// 支付卡号
        /// </summary>
        public string PaymentCardNumber { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }

        /// <summary>
        /// 交易地点 Code（关联 dic_place）
        /// </summary>
        public string PlaceCode { get; set; }

        /// <summary>
        /// 外部单号？（例如 PMS 上行来的）
        /// </summary>
        public string CheckNumber { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsVoild { get; set; }

        /// <summary>
        /// 批次号（RequestId）
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// 手动或自动调整卡值
        /// </summary>
        public bool IsManual { get; set; }

        /// <summary>
        /// 流水所属交易 Id
        /// </summary>
        public long? TransactionId { get; set; }

        /// <summary>
        /// 流水所属卡交易 Id
        /// </summary>
        public long? MembershipCardTransactionId { get; set; }

        /// <summary>
        /// 附属卡Id
        /// </summary>
        public long? SubMembershipCardId { get; set; }

        /// <summary>
        /// 渠道 code（dic_member_source的code）
        /// </summary>
        public string SourceCode { get; set; }

        /// <summary>
        /// 该笔流水是否已经计算过储值成长值
        /// </summary>
        public bool? IsStoredGrowthCalculated { get; set; }

        public virtual Transaction Transaction { get; set; }

        public virtual Account Account { get; set; }

        public virtual MembershipCardAccount MembershipCardAccount { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }

    }
}
