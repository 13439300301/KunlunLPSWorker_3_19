using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.RoomCouponRules
{
   public class RoomCouponsRuleCouponsDetail : BaseEntity
    {
        public long Id { get; set; }

        public long RoomCouponsRuleId { get; set; }

        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }

        public virtual RoomCouponsRule RoomCouponsRule { get; set; }
    }
}
