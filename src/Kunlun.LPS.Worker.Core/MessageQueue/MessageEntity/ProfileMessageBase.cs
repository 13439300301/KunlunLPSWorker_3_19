using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class ProfileMessageBase : BaseMessage
    {
        /// <summary>
        /// 会员档案id
        /// </summary>
        public long ProfileId { get; set; }
    }
}
