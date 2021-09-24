using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class DicSourceSvc : ConfigurationBase
    {
        public string PosId { get; set; }

        public string HotelCode { get; set; }

        public int SortId { get; set; }
    }
}
