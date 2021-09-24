using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_Change)]
    public class StoredValueChangeWechatMessage:BaseMessage
    {
        public string OpenId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Type { get; set; }

        public StoredValueChangeWechatMessageDetail Detail { get; set; }
    }
    public class StoredValueChangeWechatMessageDetail
    {
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionTime { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TransactionType { get; set; }

        public string Amount { get; set; }

        public string Balance { get; set; }

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
