using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class StoredValuePaymentPointsRulePlaceMap:BaseEntity
    {
        public long Id { get; set; }

        public long StoredValuePaymentPointsRuleId { get; set; }

        public string PlaceCode { get; set; }
    }
}
