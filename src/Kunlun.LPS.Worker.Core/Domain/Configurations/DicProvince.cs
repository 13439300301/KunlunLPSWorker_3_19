using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class DicProvince : ConfigurationBase
    {
        public string CountryCode { get; set; }

        public int? SortId { get; set; }
    }
}
