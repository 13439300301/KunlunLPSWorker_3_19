using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Consume_New)]
    public class ConsumeWechatMessage:BaseMessage
    {
        public string OpenId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Type { get; set; }

        public ConsumeMessageDetail Detail { get; set; }
    }
    public class ConsumeMessageDetail
    {
        public DateTime TransactionTime { get; set; }

        /// <summary>
        /// 业态类型名称
        /// </summary>
        public string ConsumeType { get; set; }

        /// <summary>
        /// 账单号
        /// </summary>
        public string CheckNumber { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public string Amount { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string Store { get; set; }

        /// <summary>
        /// 地点名称
        /// </summary>
        public string Outlet { get; set; }
    }
}
