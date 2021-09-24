using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class RegularValueStorageRule : BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public long MembershipCardAccountId { get; set; }

        public decimal Amount { get; set; }

        public bool IsEmpty { get; set; }

        public string Config { get; set; }
    }
}
