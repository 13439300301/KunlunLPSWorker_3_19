using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons
{
    public class MultiConsumeGiftCouponsRulePlaceMap : BaseEntity
    {
        public long Id { get; set; }

        public long MultiConsumeGiftCouponsRuleId { get; set; }

        public string PlaceCode { get; set; }

        public virtual MultiConsumeGiftCouponsRule MultiConsumeGiftCouponsRule { get; set; }
    }
}
