using Kunlun.LPS.Worker.Core.Enum;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class ImportBaseMessage : BaseMessage
    {
        public string Path { get; set; }

        public ImportType ImportType { get; set; }

        public string UserCode { get; set; }
    }
}
