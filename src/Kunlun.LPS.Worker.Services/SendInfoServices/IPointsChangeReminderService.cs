using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface IPointsChangeReminderService
    {
        void SendInfo(long pointsAccountHistoryId, string eventCode);

        void SendInfo(PointsValueMessageBase baseEntity, string eventCode);
    }
}
