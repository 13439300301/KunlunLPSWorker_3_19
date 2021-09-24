using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Model
{
    public class CustomEventConfigModel
    {
        public int? ConsumptionTimesThreshold { get; set; }

        public string Type { get; set; }

        public Register Register { get; set; }

        public NewMembershipCard NewMembershipCard { get; set; }
    }

    public class Register
    {
        public string MemberSourceCodes { get; set; }

        public string MembershipCardTypeIds { get; set; }
    }

    public class NewMembershipCard
    {
        public string MemberSourceCodes { get; set; }

        public string MembershipCardTypeIds { get; set; }
    }
}
