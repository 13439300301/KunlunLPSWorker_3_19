using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.MembershipCard_LevelChange)]
    public class MembershipCardChangeLevelMessage:BaseMessage
    {
        /// <summary>
        /// 现级别id
        /// </summary>
        public long CardLevelId { get; set; }

        /// <summary>
        /// 原级别Id
        /// </summary>
        public long SourceLevelId { get; set; }

        /// <summary>
        /// 卡Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        ///  升级Upgrade,降级Downgrade,保级Relegation
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 变更记录Id
        /// </summary>
        public long HistoryId { get; set; }
    }
}
