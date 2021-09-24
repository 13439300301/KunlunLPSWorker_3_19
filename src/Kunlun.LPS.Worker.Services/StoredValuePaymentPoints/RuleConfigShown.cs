using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.StoredValuePaymentPoints
{
    public class RuleConfigShown
    {
        public long MembershipCardLevelId { get; set; }

        public decimal? Revenue { get; set; }

        public decimal? Points { get; set; }
    }
}
