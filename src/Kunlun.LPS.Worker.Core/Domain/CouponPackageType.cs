using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponPackageType : BaseEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? Seq { get; set; }

        public string Description { get; set; }
    }
}
