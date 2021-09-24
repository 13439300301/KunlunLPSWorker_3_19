using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum CouponTransactionHistoryOperationType
    {
        /// <summary>
        /// 兑换券
        /// </summary>
        Exchange = 0,

        /// <summary>
        /// 使用券
        /// </summary>
        Use = 1,

        /// <summary>
        /// 取消兑换券
        /// </summary>
        CancelExchangeCoupon = 2,

        /// <summary>
        /// 取消使用券
        /// </summary>
        CancelUse = 3,

        /// <summary>
        /// 赠送券
        /// </summary>
        Gift = 4,

        /// <summary>
        /// 延长使用时间
        /// </summary>
        TimeExpand = 5,

        /// <summary>
        /// 过期
        /// </summary>
        Expires = 6
    }
}
