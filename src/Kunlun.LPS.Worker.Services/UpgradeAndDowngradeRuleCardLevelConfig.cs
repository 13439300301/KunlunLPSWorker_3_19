using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class UpgradeAndDowngradeRuleCardLevelConfig
    {
        public long Id { get; set; }

        public Object Code { get; set; }

        public int Level { get; set; }

        /// <summary>
        /// 升级成长值
        /// </summary>
        public decimal? Growth { get; set; }

        /// <summary>
        /// 保级成长值
        /// </summary>
        public decimal? MaintainGrowth { get; set; }

        /// <summary>
        /// 是否参与升降级
        /// </summary>
        public bool CanToThisLevel { get; set; }

    }
}
