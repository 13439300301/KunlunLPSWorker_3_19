 using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ProfileIdCouponTypeId
    {
        public long ProfileId { get; set; }

        public List<ExchangeCouponTypeModel> CouponTypeDetail { get; set; }
    }
}
