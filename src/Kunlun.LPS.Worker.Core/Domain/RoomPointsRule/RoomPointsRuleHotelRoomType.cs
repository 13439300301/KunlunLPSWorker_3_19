using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRuleHotelRoomType : BaseEntity
    {
        public long Id { get; set; }
        public long RoomPointsRuleId { get; set; }
        public string HotelCode { get; set; }
        public string RoomTypeCode { get; set; }
        public virtual RoomPointsRules RoomPointsRule { get; set; }
    }
}
