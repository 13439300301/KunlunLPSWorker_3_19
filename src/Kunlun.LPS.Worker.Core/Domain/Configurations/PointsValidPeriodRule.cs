using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class PointsValidPeriodRule: ConfigurationBase
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long PointsAccrueTypeId { get; set; }

        public string Config { get; set; }

        public string Description { get; set; }
    }
}
