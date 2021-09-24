using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
   public class RechargeAmountUpgradeRule: BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }

    }
}
