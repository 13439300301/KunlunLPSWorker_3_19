using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public class RegisterPointsMessage
    {
        /// <summary>
        /// 会员卡 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 会员档案id
        /// </summary>
        public long ProfileId { get; set; }
    }
}
