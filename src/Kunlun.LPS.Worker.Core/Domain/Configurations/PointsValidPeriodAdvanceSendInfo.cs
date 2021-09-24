using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class PointsValidPeriodAdvanceSendInfo : ConfigurationBase
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public string Config { get; set; }
    }
}
