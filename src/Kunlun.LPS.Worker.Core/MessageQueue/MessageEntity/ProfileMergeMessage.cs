using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Profile_Merge)]
    public class ProfileMergeMessage : BaseMessage
    {
        public List<long> TransactionIds { get; set; }

    }
}