using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class AnonymousEnrollmentMessage : BaseMessage
    {
        public List<long> MembershipCardIds { get; set; }

    }
}
