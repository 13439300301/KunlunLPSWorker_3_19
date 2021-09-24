using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.MembershipCardLevelChange_Upgrade)]
    public class UpdateProfileMessage : BaseMessage
    {
        /// <summary>
        /// 会员卡id
        /// </summary>
        public long MembershipCardId { get; set; }
        /// <summary>
        /// 当前卡级别id
        /// </summary>
        public long CardLevelId { get; set; }
        /// <summary>
        ///  级别变更方向
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MembershipCardLevelChangeType Direction { get; set; }
    }
}
