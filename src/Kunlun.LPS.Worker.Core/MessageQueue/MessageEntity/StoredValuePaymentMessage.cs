using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    /// <summary>
    /// 卡值支付消息
    /// </summary>
    [RoutingKey(RoutingKeys.StoredValue_Payment)]
    public class StoredValuePaymentMessage : StoredValueMessageBase
    {

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }


        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// 卡余额
        /// </summary>
        public decimal Balance { get; set; }
    }
}
