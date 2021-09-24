using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.DivisionCards
{
    public class FailCardTransaction : BaseEntity
    {
        public long Id { get; set; }

        public long? LeaderMembershipCardTransactionId { get; set; }

        public long? LeaderStoredValueHistoryId { get; set; }
    }
}
