using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class Sysparam : ConfigurationBase
    {
        public string ParValue { get; set; }

        public string Chname { get; set; }

        public string Engname { get; set; }

        public string Des { get; set; }

        public string HotelCode { get; set; }

        public string ControlType { get; set; }

        public string ControlSql { get; set; }
    }
}
