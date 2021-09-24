using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_ToTransfer)]
    public class StoredValueToTransferMessage:StoredValueMessageBase
    {
        /// <summary>
        /// 转账会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
