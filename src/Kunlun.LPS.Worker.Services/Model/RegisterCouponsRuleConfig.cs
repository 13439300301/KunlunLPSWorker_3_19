using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Model
{
    public class RegisterCouponsRuleConfig
    {
        public List<RegisterCouponsDetail> RegisterCouponsDetail { get; set; }
    }

    public class RegisterCouponsDetail
    {
        public long CouponTypeId { get; set; }

        public int Quantity { get; set; }
        public int? RegisteredPlacesBegin { get; set; }
        public int? RegisteredPlacesEnd { get; set; }
    }
}
