using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    /// <summary>
    /// 会员绑定新卡消息
    /// </summary>
    [RoutingKey(RoutingKeys.Profile_MembershipCardBind)]
    public class MembershipCardBindMessage : BaseMessage
    {
        /// <summary>
        /// 会员卡 Id
        /// </summary>
        [Display(Name = "会员卡Id")]
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        [Display(Name = "会员卡号")]
        public string MembershipCardNumber { get; set; }


        /// <summary>
        /// 会员卡所属 Profile 的 Id
        /// </summary>
        [Display(Name = "会员卡所属 Profile 的 Id")]
        public long ProfileId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        public DateTime? ExpireDate { get; set; }
        public string UserCode { get; set; }
    }
}
