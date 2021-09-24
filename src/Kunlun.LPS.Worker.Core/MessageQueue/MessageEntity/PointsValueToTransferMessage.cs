using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_ToTransfer)]
    public class PointsValueToTransferMessage : PointsValueMessageBase
    {
        public string MembershipCardNumber { get; set; }
    }
}
