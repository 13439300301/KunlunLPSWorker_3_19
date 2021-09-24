using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    /// <summary>
    /// 要兑换的券列表
    /// </summary>
    public class CouponListDetail
    {
        /// <summary>
        /// 券类型id
        /// </summary>
        public long CouponTypeId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? Unitprice { get; set; }
    }
}
