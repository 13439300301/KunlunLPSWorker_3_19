using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class RechargeActivityRule : BaseEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// 卡类型id
        /// </summary>
        public long? MembershipCardTypeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 是否有效（默认有效）
        /// </summary>
        public bool IsAvailable { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 配置 `{ points 积分: { amount 每充值金额, points 奖励积分 }, growth 成长值: { amount, growth 奖励成长值 } }`
        /// </summary>
        public string Config { get; set; }

    }
}
