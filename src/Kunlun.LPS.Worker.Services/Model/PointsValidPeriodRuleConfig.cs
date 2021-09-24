using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Model
{
    public class PointsValidPeriodRuleConfig
    {
        public string type { get; set; }

        public EveryPointsHistory everyPointsHistory { get; set; }

        public FixedDate fixedDate { get; set; }

        public AdvanceSendInfo advanceSendInfo { get; set; }
    }

    public class EveryPointsHistory
    {
        public int value { get; set; }

        public string unit { get; set; }

        public bool IsLastDayOfMonth { get; set; } 
    }

    public class FixedDate
    {
        public int year { get; set; }

        public string date { get; set; }
    }

    public class AdvanceSendInfo
    {
        public List<Templates> templates { get; set; }

        public Frequency frequency { get; set; }
    }

    public class Templates
    {
        public string type { get; set; }

        public long id { get; set; }
    }

    public class Frequency
    {
        public int day { get; set; }

        public int nextMonth { get; set; }
    }
}