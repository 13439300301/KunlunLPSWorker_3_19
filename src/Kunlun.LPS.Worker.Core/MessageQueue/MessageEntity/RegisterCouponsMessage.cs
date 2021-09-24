using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Coupon_RegisterGift)]
    public class RegisterCouponsMessage : BaseMessage
    {
        /// <summary>
        /// 档案 Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 会员卡 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 会员卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 会员卡级别 Id
        /// </summary>
        public long MembershipCardLevelId { get; set; }

        /// <summary>
        /// 注册渠道
        /// </summary>
        public string MemberSource { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }

        public string UserCode { get; set; }  
    }
}
