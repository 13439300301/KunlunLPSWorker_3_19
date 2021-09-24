using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class StoredValueMessageBase : BaseMessage
    {
        /// <summary>
        /// 档案ID
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 会员卡 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 业务单 Id（可以唯一标识本次充值动作）
        /// </summary>
        public long TransactionId { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 卡余额
        /// </summary>
        public decimal Balance { get; set; }
    }
}
