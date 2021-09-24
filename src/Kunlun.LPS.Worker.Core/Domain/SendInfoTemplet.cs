using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class SendInfoTemplet : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Doc { get; set; }

        public string Valid { get; set; }

        public DateTime CreateDate { get; set; }

        public string Creator { get; set; }

        public string Content { get; set; }

        public string TempletType { get; set; }

        public string Ext { get; set; }

        public string Title { get; set; }

        public string UsageCode { get; set; }

        public string MessageSupplierCode { get; set; }

        public string MessageChannel { get; set; }

        public string MembershipType { get; set; }
    }
}
