using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 余额下限提醒明细
    /// </summary>
    public class MembershipCardBalanceNotificationDetail : BaseEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// 余额下限Id
        /// </summary>
        public long NotificationId { get; set; }

        /// <summary>
        /// 提醒类型，0 None，1 Email，2 SMS 短信
        /// </summary>
        public BalanceNotificationDetailType Type { get; set; }

        /// <summary>
        /// 接收人 手机号、邮箱
        /// </summary>

        public string Addressee { get; set; }

    }
}
