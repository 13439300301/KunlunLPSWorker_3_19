using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class MembershipCardLevel: ConfigurationBase
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; } 

        public int Level { get; set; }

        public string Description { get; set; }

        public decimal? DefaultBalance { get; set; } 
    }
}
