using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.MembershipCard_ChangeLevel)]
    public class CardLevelChangeWechatMessage:BaseMessage
    {
        public string OpenId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Type { get; set; }

        public CardLevelChangeMessageDetail Detail { get; set; }
    }
    public class CardLevelChangeMessageDetail
    {
        public DateTime Time { get; set; }

        /// <summary>
        /// 级别变化方向
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 原卡级别名称
        /// </summary>
        public string SourceLevel { get; set; }

        /// <summary>
        /// 升级后级别名称
        /// </summary>
        public string DestinationLevel { get; set; }

        public string Description { get; set; }
    }

    public enum LevelChangeDirection
    {
        /// <summary>
        /// 升级
        /// </summary>
        Upgrade,

        /// <summary>
        /// 降级
        /// </summary>
        Downgrade,

        /// <summary>
        /// 保级
        /// </summary>
        Relegation
    }
}
