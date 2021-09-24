using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.Coupons
{
    public interface ICouponService
    {
        void GiftCoupons(RegisterCouponsMessage message, Dictionary<long, int> exchangeCoupons, long? couponChannel, string placeCode, DateTime date);

        void UpdateCouponInventory(CouponUpdateInventoryMessage message);

        void BatchGiftCoupons(BatchGiftCouponMessage message);
    }
}
