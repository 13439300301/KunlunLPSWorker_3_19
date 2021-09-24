using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class ProfileCommonMessage
    {
        public long ProfileId { get; set; }

        public long MembershipCardId { get; set; }

        public string Password { get; set; }

        public decimal? Balance { get; set; }

        public decimal? Points { get; set; }

        public DateTime? EnrollDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
