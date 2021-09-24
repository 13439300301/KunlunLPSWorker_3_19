using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface IBalanceNotificationService
    {
        void SendBalanceNotification(StoredValueMessageBase message,string eventCode);
    }
}
