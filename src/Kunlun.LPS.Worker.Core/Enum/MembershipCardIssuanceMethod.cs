using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum MembershipCardIssuanceMethod
    {
        /// <summary>
        /// 绑定新增卡
        /// </summary>
        /// [EnumMember(Value = "ffp")]
        Binding = 1,

        /// <summary>
        /// 购买新增卡
        /// </summary>
        Purchase = 2,

        /// <summary>
        /// 不记名批量新增卡
        /// </summary>
        Batch = 3,
    }
}
