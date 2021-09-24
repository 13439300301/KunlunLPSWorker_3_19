using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_UpdateInventory)]
    public class CouponUpdateInventoryMessage : BaseMessage
    {
        /// <summary>
        /// 券类型id
        /// </summary>
        public long CouponTypeId { get; set; }

        /// <summary>
        /// 是否管理库存
        /// </summary>
        public bool NeedManageInventory { get; set; }

        /// <summary>
        /// 数量（大于0加，小于零减）
        /// </summary>
        public int Inventory { get; set; }

        /// <summary>
        /// 是否是扣库存
        /// </summary>
        public bool IsDeductInventory { get; set; }
    }
}
