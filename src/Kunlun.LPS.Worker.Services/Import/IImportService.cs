using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;

namespace Kunlun.LPS.Worker.Services.Import
{
    public interface IImportService
    {
        void ImportConsume(ImportConsumeMessage message);
    }
}
