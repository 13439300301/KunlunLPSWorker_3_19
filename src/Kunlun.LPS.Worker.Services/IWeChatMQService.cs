using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public interface IWeChatMQService
    {
        void PublishWeChatMQ(StoredValueMessageBase storedValueChangeWechatMessage);

        void PublishWeChatMQ(PointsValueMessageBase pointsChangeWechatMessage);

        void PublishWeChatMQ(ConsumeNewMessage consumeNewMessage);

        void PublishWeChatMQ(MembershipCardChangeLevelMessage membershipCardChangeLevelMessage);
    }
}
