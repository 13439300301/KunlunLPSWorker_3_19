using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class CommonWechatMessage<T> :BaseMessage
    {
        public string OpenId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public WeixinMessageType Type { get; set; }

        public T Detail { get; set; }
    }

    public enum WeixinMessageType
    {
        StoredValue,
        Points,
        LevelChange,
        Consume
    }
}
