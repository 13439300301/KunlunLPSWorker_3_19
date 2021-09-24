using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class MembershipCardType : ConfigurationBase
    {
        public long Id { get; set; }

        public bool IsLimitBalance { get; set; }

        public decimal? LimitBalance { get; set; }
        public decimal? RealNameLimitBalance { get; set; }
        public bool HasValidPeriod { get; set; }
    }
}
