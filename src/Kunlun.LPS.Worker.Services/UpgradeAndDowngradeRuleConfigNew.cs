using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class UpgradeAndDowngradeRuleConfigNew
    {
        /// <summary>
        /// 会员卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 允许降级
        /// </summary>
        public bool AllowDowngrade { get; set; }

        /// <summary>
        /// 允许跨级降级
        /// </summary>
        public bool AllowSkipDowngrade { get; set; }

        /// <summary>
        /// 允许跨级升级
        /// </summary>
        public bool AllowSkipUpgrade { get; set; }

        /// <summary>
        /// 跨级升级时必须是一笔消费满足成长值（只有勾选了“允许跨级”才能选）
        /// </summary>
        public bool AllowSkipUpgradeOnlyOneConsumption { get; set; }

        /// <summary>
        /// 动态考察期：原考察期结束后根据当前日期减去1年来作为新的考察期（只有没勾选“允许降级”才能选）
        /// </summary>
        public bool DynamicRetentionPeriod { get; set; }

        /// <summary>
        /// 升级时加入卡级别的成长值
        /// </summary>
        public bool AddCardLevelGrowthWhenUpgrade { get; set; }

        /// <summary>
        /// 允许升级保留溢出成长值
        /// </summary>
        public bool AllowUpgradeRetainSpilloverGrowth { get; set; }

        /// <summary>
        /// 级别设置
        /// </summary>
        public List<UpgradeAndDowngradeRuleCardLevelConfig> Levels { get; set; }

        /// <summary>
        /// 考察期设置
        /// </summary>
        public UpgradeAndDowngradeRuleRetentionPeriodConfig RetentionPeriod { get; set; }

    }

    public class UpgradeAndDowngradeRuleRetentionPeriodConfig
    {
        public string Mode { get; set; }

        public string Value { get; set; }
    }

    /// <summary>
    /// 考察期计算方式
    /// </summary>
    public enum UpgradeAndDowngradeRuleRetentionPeriodMode
    {
        /// <summary>
        /// 按月
        /// </summary>
        Month = 0,

        /// <summary>
        /// 按自然年
        /// </summary>
        Year,

        /// <summary>
        /// 按注册日
        /// </summary>
        RegistrationDate
    }
}
