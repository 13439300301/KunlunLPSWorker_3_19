using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomFirstStayGiftPointForBucket : BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleCardLevelDetailForBucketId { get; set; }

        public bool IsFirstStayGiftPoints { get; set; } 


        public virtual RoomPointsRuleForBucketMembershipCardLevel RoomPointsRuleForBucketMembershipCardLevel { get; set; }

        public decimal Points { get; set; }
    }
}
