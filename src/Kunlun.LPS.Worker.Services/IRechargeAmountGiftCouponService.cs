using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public interface IRechargeAmountGiftCouponService
    {
        public void RechargeAmountGiftCoupon(RechargeAmountGiftCouponMessage message);
    }
}
