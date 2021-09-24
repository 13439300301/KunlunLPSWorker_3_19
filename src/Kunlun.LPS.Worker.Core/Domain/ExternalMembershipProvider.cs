using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ExternalMembershipProvider : BaseEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ExternalMembershipProviderHandler Type { get; set; }
    }
}
