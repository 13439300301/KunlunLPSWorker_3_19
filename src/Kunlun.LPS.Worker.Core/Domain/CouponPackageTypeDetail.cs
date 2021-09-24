using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponPackageTypeDetail : BaseEntity
    {
        public long Id { get; set; }

        public long CouponPackageTypeId { get; set; }

        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }

        public virtual CouponPackageType CouponpackageType { get; set; }

        public virtual CouponType CouponType { get; set; }
    }
}
