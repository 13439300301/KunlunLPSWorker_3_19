using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class ProfileResetPasswordMessage : ProfileMessageBase
    {
        public string NewPwd { get; set; }
    }
}
