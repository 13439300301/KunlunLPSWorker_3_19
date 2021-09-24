using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class RegisterCouponsRule : ConfigurationBase
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }
        public bool ShouldGiftNewCard { get; set; }

        public string Description { get; set; }

        public int? Several { get; set; }
        public string Config { get; set; }
        public NameStatisticalTimeUnit? NameStatisticalTimeUnit { get; set; }
    }
}
