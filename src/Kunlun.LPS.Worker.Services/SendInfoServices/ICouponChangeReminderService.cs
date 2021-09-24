using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface ICouponChangeReminderService
    {
        void SendInfo(RegisterCouponsMessage message, List<CouponTransactionHistory> list, DateTime date, string eventCode);
    }
}
