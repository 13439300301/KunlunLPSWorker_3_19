using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.StoredValue_InitCardFee)]
    public class StoredValueInitCardFeeMessage : StoredValueMessageBase
    {
        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }
    }
}
