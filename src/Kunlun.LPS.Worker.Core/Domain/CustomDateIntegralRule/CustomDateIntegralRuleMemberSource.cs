using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.CustomDateIntegralRule
{
    public class CustomDateIntegralRuleMemberSource : BaseEntity
    {
        public long Id { get; set; }

        public long CustomDateIntegralRulesId { get; set; }

        public string MemberSourceCode { get; set; }

        public virtual CustomDateIntegralRules CustomDateIntegralRules { get; set; }
    }
}
