using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    /// <summary>
    /// 卡值变动发SendInfo
    /// </summary>
    public interface IStoredValueChangeReminderService
    {
        public void SendInfo(StoredValueMessageBase message,string eventCode);
    }
}
