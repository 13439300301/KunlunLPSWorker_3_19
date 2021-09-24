﻿using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Core.Domain.RoomPointsRule
{
    public class RoomPointsRuleForBucketMembershipCardLevel: BaseEntity
    {
        public long Id { get; set; }

        public long RoomPointsRuleId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public string BucketCode { get; set; }

        public decimal Revenue { get; set; }

        public decimal Points { get; set; }



        public virtual RoomPointsRules RoomPointsRule { get; set; }
    }
}
