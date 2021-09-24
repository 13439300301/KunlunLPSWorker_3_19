using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum StoredValueAccountHistoryOperationType
    {
        /// <summary>
        /// 储值
        /// </summary>
        RenewalFee = 0,

        /// <summary>
        /// 退费
        /// </summary>
        RefundFee = 1,

        /// <summary>
        /// 预授权
        /// </summary>
        Preauthorization = 2,

        /// <summary>
        /// 消费支付
        /// </summary>
        CheckOut = 3,

        /// <summary>
        /// 赠送
        /// </summary>
        Rewards = 4,

        /// <summary>
        /// 转账
        /// </summary>
        Transfer = 5,

        /// <summary>
        /// 冲账（支付退费）,取消冲账
        /// </summary>
        Redemption = 6,

        /// <summary>
        /// 调整
        /// </summary>
        Adjust = 7,

        /// <summary>
        /// 取消奖励（赠送退费）
        /// </summary>
        CancelRewards = 8,

        /// <summary>
        /// 过期
        /// </summary>
        Expired = 9,

        /// <summary>
        /// 合并会员卡
        /// </summary>
        MergeMembershipCard = 10,

        /// <summary>
        /// 还款
        /// </summary>
        Repayment = 11,

        /// <summary>
        /// 初始化剩余透支额度
        /// </summary>
        InitOverdraft = 12,

        /// <summary>
        /// 透支额度调整
        /// </summary>
        CreditLineAdjust = 13,

        /// <summary>
        /// 分裂卡
        /// </summary>
        DivisionCard = 14,

        /// <summary>
        /// 初始卡值
        /// </summary>
        InitCardFee = 15,

        /// <summary>
        /// 介绍人赠送卡值
        /// </summary>
        IntroducerPresentsFee = 16,


    }
}
