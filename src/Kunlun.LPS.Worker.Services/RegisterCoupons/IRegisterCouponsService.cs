using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.RegisterCoupons
{
    public interface IRegisterCouponsService
    {
        void GiftCoupons(RegisterCouponsMessage message, bool newCard);
    }
}
