using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class RegisterPointsRuleMemberSourceMap : BaseEntity
    {
        public long Id { get; set; }

        public long RegisterPointsRuleId { get; set; }

        public string MemberSource { get; set; }

        public virtual RegisterPointsRule RegisterPointsRule { get; set; }
    }
}
