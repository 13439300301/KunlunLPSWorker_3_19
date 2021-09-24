using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class RechargeAmountUpgradeRuleDetail: BaseEntity
    {
        public long Id { get; set; }

        public long? RechargeAmountUpgradeRuleId { get; set; }

        public long CurrentCardLevelId { get; set; }

        public long UpgradeCardLevelId { get; set; }

        public decimal MinimumLimitAmount { get; set; }

        public decimal MaxLimitAmount { get; set; }

        public virtual RechargeAmountUpgradeRule RechargeAmountUpgradeRule { get; set; }
    }
}
