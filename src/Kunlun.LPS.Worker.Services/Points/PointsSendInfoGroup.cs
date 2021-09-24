using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class PointsSendInfoGroup
    {
        public List<string> expireDate { get; set; }
        public string membershipCardNumber { get; set; }
        public decimal remainingPoints { get; set; }
        public string sendInfoConfig { get; set; }
        public long membershipCardTypeId { get; set; }
    }
}
