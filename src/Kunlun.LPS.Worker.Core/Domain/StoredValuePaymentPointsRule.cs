using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class StoredValuePaymentPointsRule:BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Config { get; set; }

        public string Description { get; set; }
    }
}
