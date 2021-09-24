using Kunlun.LPS.Worker.Core.Domain.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class PointsHistoryDetail : BaseVersionedEntity
    {
        public long Id { get; set; }

        public long MembershipCardId { get; set; }

        public long AccountId { get; set; }

        public long PointsAccountHistoryId { get; set; }

        public long? PointsHistoryDetailId { get; set; }

        public DateTime ExpireDate { get; set; }

        public decimal Points { get; set; }

        public decimal RemainingPoints { get; set; }

        /// <summary>
        /// 共享积分账户定义Id
        /// </summary>
        public long? SharedPointsAccountId { get; set; }

        public virtual PointsAccountHistory PointsAccountHistory { get; set; }
    }
}