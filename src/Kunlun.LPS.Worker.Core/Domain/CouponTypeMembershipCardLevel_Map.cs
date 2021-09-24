﻿using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponTypeMembershipCardLevel_Map : BaseEntity
    {
        public long Id { get; set; }

        public long CouponTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public virtual CouponType CouponType { get; set; }
    }
}
