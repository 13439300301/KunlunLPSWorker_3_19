using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class RegisterPointsRule : BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public string WeekControl { get; set; }

        public decimal Limit { get; set; }

        public ExpireMode ExpireMode { get; set; }

        public int? Expires { get; set; }

        public string Description { get; set; }

        public long TargetMembershipTypeAccountId { get; set; }

        public bool ShouldRewardNewCard { get; set; }
    }
}
