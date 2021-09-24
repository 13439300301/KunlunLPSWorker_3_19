using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class RegisterPointsRuleMembershipCardLevelDetail : BaseEntity
    {
        public long Id { get; set; }

        public long RegisterPointsRuleId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public decimal Points { get; set; }

        public virtual RegisterPointsRule RegisterPointsRule { get; set; }
    }
}
