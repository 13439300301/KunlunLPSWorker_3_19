using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface IProfileSendInfoService
    {
        void SendInfo(ProfileCommonMessage message, string eventCode);
    }
}
