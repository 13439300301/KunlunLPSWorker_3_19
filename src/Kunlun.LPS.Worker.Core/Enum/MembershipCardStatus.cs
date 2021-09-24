using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum MembershipCardStatus
    {
        /// <summary>
        /// 未入库
        /// </summary>
        InTransit = 0,

        /// <summary>
        /// 已入库
        /// </summary>
        InStock = 1,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 冻结
        /// </summary>
        Freeze = 3,

        /// <summary>
        /// 销卡
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// 待销售
        /// </summary>
        PendingSales = 5,
        /// <summary>
        /// 未开卡
        /// </summary>
        NotOpenCard = 6
    }
}
