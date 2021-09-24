using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    /// <summary>
    /// 取消充值（退费）
    /// </summary>
    [RoutingKey(RoutingKeys.StoredValue_TopupRefund)]
    public class TopupRefundMessage : StoredValueMessageBase
    {

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 业务单号（可以唯一标识本次取消充值动作）
        /// </summary>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// 退款金额（参与了充100送20的活动，现在要退款，需要退100元给会员，同时卡内金额减少120，120即退款金额，100即本金）
        /// </summary>
        public decimal RefoundAmount { get; set; }

        /// <summary>
        /// 卡余额
        /// </summary>
        public decimal Balance { get; set; }
    }
}
