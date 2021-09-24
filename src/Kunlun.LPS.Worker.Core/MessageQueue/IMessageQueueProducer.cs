using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Core.MessageQueue
{
    public interface IMessageQueueProducer
    {
        void PublishWechat<T>(T content, Dictionary<string, object> headers = null) where T : BaseMessage;

        void PublishInternal<T>(T content, Dictionary<string, object> headers = null) where T : BaseMessage;
    }
}
