using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class MealPeriod : ConfigurationBase
    {
        public long Id { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public bool IsNextDay { get; set; }
    }
}
