using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.NotificationServices
{
    public interface IPointsValueNotificationService
    {
        void SendPointsChangeNotification(PointsValueMessageBase messageBase);

        void SendPointsChangeNotification(PointsValueDailyCheckInBonusMessage pointsValueDailyCheckInBonusMessage);
    }
}
