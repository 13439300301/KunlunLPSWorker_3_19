using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class UserNotification : BaseEntity
    {
        public long Id { get; set; }

        public NotificationType Type { get; set; }

        public string Content { get; set; }

        public string UserCode { get; set; }

        public bool AlreadyRead { get; set; }
    }
}
