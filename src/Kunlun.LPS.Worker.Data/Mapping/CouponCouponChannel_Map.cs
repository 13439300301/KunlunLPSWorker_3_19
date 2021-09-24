using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponCouponChannel_Map : BaseEntity
    {
        public long Id { get; set; }

        public long CouponTypeId { get; set; }

        public long CouponChannelId { get; set; }

        public virtual CouponType CouponType { get; set; }
    }
}
