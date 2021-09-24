using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SFTPMembership
{
    public class SFTPMembershipInfo
    {
        public string MemberNo { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string MemberType { get; set; }

        public string CardStatus { get; set; }

        public string MemValidThru { get; set; }
    }
}
