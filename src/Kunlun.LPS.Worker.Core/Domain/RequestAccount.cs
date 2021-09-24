using Kunlun.LPS.Worker.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class RequestAccount : BaseVersionedEntity
    {
        public List<Account> Accounts { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
