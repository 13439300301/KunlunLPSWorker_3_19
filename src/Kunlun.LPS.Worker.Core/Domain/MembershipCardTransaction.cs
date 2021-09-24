using Kunlun.LPS.Worker.Core.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 会员卡交易表（交易是指当前进行卡值或积分操作）
    /// </summary>
    public class MembershipCardTransaction : BaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 该会员卡交易所属的交易 Id
        /// </summary>
        public long TransactionId { get; set; }

        /// <summary>
        /// 当前交易的卡id（有可能是附属卡支付，此时写的就是附属卡id）
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 这笔交易发生在该卡上的总金额（不包含透支账户）
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///  实收金额
        /// </summary>
        public decimal RealAmount { get; set; }

        /// <summary>
        /// 该会员卡上次余额（不包含透支账户）
        /// </summary>
        public decimal LastBalance { get; set; }

        /// <summary>
        /// 该会员卡本次余额（不包含透支账户）
        /// </summary>
        public decimal ThisBalance { get; set; }

        /// <summary>
        /// 交易积分
        /// </summary>
        public decimal Points { get; set; } = 0;

        /// <summary>
        /// 主卡id，一定写值
        /// </summary>
        public long? MainMembershipCardId { get; set; }

        public virtual Transaction Transaction { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }

    }
}
