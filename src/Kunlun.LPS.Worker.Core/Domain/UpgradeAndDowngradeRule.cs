using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class UpgradeAndDowngradeRule: BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public int RetentionPeriod { get; set; }

        public string Description { get; set; }

        public string Config { get; set; }
    }
}
