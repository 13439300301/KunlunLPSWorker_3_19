using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponTypePaymentWay_Map : BaseEntity
    {

        public long Id { get; set; }

        public long CouponTypeId { get; set; }

        public long PaymentWayId { get; set; }


        public virtual CouponType CouponType { get; set; }

        public virtual CouponTypePaymentWay PaymentWay { get; set; }
    }
}
