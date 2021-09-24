using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.RegisterPoints
{
    public interface IRegisterPointsService
    {
        void GiftPoints(RegisterPointsMessage message, string routingKeys);
    }
}
