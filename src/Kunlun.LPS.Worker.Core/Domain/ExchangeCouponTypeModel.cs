using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ExchangeCouponTypeModel
    {
        public long CouponTypeId { get; set; }

        public int Count { get; set; }

        public decimal? UnitPrice { get; set; }

        public string SerialNumberPrefix { get; set; }
        public string SerialNumber { get; set; }
    }
}
