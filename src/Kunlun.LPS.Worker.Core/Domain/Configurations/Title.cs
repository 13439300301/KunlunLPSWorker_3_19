using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class Title : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? SortId { get; set; }

        public string Gender { get; set; }
    }
}
