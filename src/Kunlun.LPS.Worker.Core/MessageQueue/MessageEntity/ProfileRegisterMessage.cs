using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Profile_Register)]
    public class ProfileRegisterMessage : ProfileMessageBase
    {
        /// <summary>
        /// 会员卡 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 卡值
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public decimal? Points { get; set; }

        public DateTime? EnrollDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UserCode { get; set; } 
    }
}
