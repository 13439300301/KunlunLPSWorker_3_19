using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponTypePaymentWay : BaseEntity
    {
        public long Id { get; set; }

        public string HotelCode { get; set; }

        public string PaymentWayCode { get; set; }

        public string PaymentWayName { get; set; }

        public int PaymentWay { get; set; }
    }
}
