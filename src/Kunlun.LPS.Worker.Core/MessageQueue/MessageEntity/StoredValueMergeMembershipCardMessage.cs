using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_MergeMembershipCard)]
    public class StoredValueMergeMembershipCardMessage:StoredValueMessageBase
    {
        /// <summary>
        /// 源会员档案Id
        /// </summary>
        public long SProfileId { get; set; }
        /// <summary>
        /// 目标会员档案Id
        /// </summary>
        public long OProfileId { get; set; }

        public List<long> TransactionIds { get; set; }
    }
}
