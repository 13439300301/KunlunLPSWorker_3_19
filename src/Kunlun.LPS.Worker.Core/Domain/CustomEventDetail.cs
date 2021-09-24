using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CustomEventDetail : BaseEntity
    {
        public long Id { get; set; }

        public int EventId { get; set; }

        public string Config { get; set; }
        public virtual CustomEvent CustomEvent { get; set; }
    }
}
