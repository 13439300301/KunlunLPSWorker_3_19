using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.PointsValue_Change)]
    public class PointsChangeWechatMessage:BaseMessage
    {
        public string OpenId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Type { get; set; }

        public PointsChangeMessageDetail Detail { get; set; }
    }
    public class PointsChangeMessageDetail
    {
        public DateTime TransactionTime { get; set; }

        public string TransactionType { get; set; }

        public string Points { get; set; }

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
