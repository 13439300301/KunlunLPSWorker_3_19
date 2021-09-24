using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_Topup)]
    public class TopupMessage : StoredValueMessageBase
    {

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 到账金额（有活动充100送20，实收100，赠送20，到账120元）
        /// </summary>
        public decimal Amount { get; set; }

        public decimal RealAmount { get; set; }

        /// <summary>
        /// 卡余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionTime { get; set; }

        public string PlaceCode { get; set; }
    }
}
