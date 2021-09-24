using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ConsumeHistoryMetadata : BaseEntity
    {
        public long Id { get; set; }
        public long HistoryId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
