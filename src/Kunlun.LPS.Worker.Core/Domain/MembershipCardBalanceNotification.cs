using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 余额下限提醒
    /// </summary>
    public class MembershipCardBalanceNotification:BaseEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// FK_MembershipCard_Id会员卡Id
        /// </summary>
        [Display(Name = "FK_MembershipCard_Id会员卡Id")]
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 下限值，小于该值触发提醒
        /// </summary>
        [Display(Name = "下限值，小于该值触发提醒")]
        public decimal Balance { get; set; }

        /// <summary>
        /// 邮件模板Id
        /// </summary>
        [Display(Name = "邮件模板Id")]
        public int? EmailTemplate { get; set; }

        /// <summary>
        /// 短信模板Id
        /// </summary>
        [Display(Name = "短信模板Id")]
        public int? SMSTemplate { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }
    }
}
