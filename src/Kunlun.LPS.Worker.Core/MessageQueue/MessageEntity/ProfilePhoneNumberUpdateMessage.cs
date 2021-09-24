using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Profile_ProfilePhoneNumberUpdate)]
    public class ProfilePhoneNumberUpdateMessage : BaseMessage
    {
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string NewPhoneNumber { get; set; }

        public long? ProfileId { get; set; }

        public string OldMobilePhoneNumber { get; set; }
    }
}
