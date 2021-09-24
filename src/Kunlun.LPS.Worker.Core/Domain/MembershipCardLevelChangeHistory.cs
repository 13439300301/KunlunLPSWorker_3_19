using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class MembershipCardLevelChangeHistory : BaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 会员 Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 该会员卡的 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 该卡的卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 原始卡级别 Id
        /// </summary>
        public long SourceLevelId { get; set; }

        /// <summary>
        /// 目标卡级别 Id
        /// </summary>
        public long DestinationLevelId { get; set; }

        /// <summary>
        /// 级别变更方向枚举（Upgrade 升级、Downgrade 降级、Relegation 保级）
        /// </summary>
        public MembershipCardLevelChangeDirection Direction { get; set; }

        /// <summary>
        /// 级别变更时间
        /// </summary>
        public DateTime ChangeTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } 
    }
}
