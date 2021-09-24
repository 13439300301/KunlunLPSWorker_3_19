using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ExternalMembershipLevelMembershipCardLevelMap : BaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 外部会员级别 Id
        /// </summary>
        public long ExternalMembershipLevelId { get; set; }

        /// <summary>
        /// 会员卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 会员卡级别 Id
        /// </summary>
        public long MembershipCardLevelId { get; set; }
    }
}
