using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.GiftCouponServices
{
    public interface IFbConsumeGiftCouponsService
    {
        void GiftCoupons(FbConsumeGiftCouponsMessage message);
    }
}
