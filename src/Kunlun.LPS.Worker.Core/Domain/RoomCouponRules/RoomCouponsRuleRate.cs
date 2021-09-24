using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.RoomCouponRules
{
   public class RoomCouponsRuleRate : BaseEntity
    {
        public long Id { get; set; }

        public long RoomCouponsRuleId { get; set; }

        public string RateCode { get; set; }

        public virtual RoomCouponsRule RoomCouponsRule { get; set; }
    }
}
