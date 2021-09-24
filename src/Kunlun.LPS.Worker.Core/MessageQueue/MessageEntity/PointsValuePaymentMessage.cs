using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_Payment)]
    public class PointsValuePaymentMessage : PointsValueMessageBase
    {
        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }
    }
}
