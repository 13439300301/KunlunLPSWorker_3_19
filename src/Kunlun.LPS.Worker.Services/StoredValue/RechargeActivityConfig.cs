using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public class RechargeActivityConfig
    {
        /// <summary>
        /// 积分
        /// </summary>
        public Model Points { get; set; }

        /// <summary>
        /// 成长值
        /// </summary>
        public Model Growth { get; set; }
    }

    public class Model
    {
        public string Id { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal GivinAmount { get; set; } = 0;
    }
}
