using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class PointsValidPeriodAdvanceSendInfoConfig
    {
        public List<Templates> Templates { get; set; }

        public Frequency Frequency { get; set; }
    }

    public class Templates
    {
        public string Type { get; set; }

        public int Id { get; set; }
    }

    public class Frequency
    {
        public int Day { get; set; }

        public int NextMonth { get; set; }
    }
}
