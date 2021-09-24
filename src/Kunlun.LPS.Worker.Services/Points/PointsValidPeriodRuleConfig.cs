using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class PointsValidPeriodRuleConfig
    {
        public string Type { get; set; }

        public EveryPointsHistory EveryPointsHistory { get; set; }

        public FixedDate FixedDate { get; set; }
    }

    public class EveryPointsHistory
    {
        public int Value { get; set; }

        public string Unit { get; set; }

        public bool IsLastDayOfMonth { get; set; } = false;
    }

    public class FixedDate
    {
        public int Year { get; set; }

        public string Date { get; set; }
    }
}
