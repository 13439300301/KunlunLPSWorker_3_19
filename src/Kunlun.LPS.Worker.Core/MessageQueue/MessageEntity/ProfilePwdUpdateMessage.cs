using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Profile_PwdUpdate)]
    public class ProfilePwdUpdateMessage : ProfileMessageBase
    {
        /// <summary>
        /// 卡密码
        /// </summary>
        public string NewPwd { get; set; }
    }
}
