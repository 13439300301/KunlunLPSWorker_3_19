using Kunlun.LPS.Worker.Core.Domain.Common;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class PointsAccountHistory : BaseVersionedEntity
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardAccountId { get; set; }

        public long MembershipCardId { get; set; }

        public long ProfileId { get; set; }

        public string MembershipCardNumber { get; set; }

        public long? TransactionId { get; set; }

        public long? MembershipCardTransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public long? HistoryId { get; set; }

        public Guid BatchId { get; set; }

        public string Description { get; set; }

        public string AccrueType { get; set; }
        
        public decimal Points { get; set; }

        public string PlaceCode { get; set; }

        public string HotelCode { get; set; }

        public decimal LastBalance { get; set; }

        public decimal ThisBalance { get; set; }

        public string FolioNo { get; set; }

        public bool IsLastCommand { get; set; }

        public bool IsAdjustPoints { get; set; }

        public long? MerchantTypeId { get; set; }

        public bool IsFee { get; set; }

        public string CheckNumber { get; set; }

        public bool IsVoid { get; set; }

        public string OperatorCode { get; set; }

        public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// 共享积分账户定义Id
        /// </summary>
        public long? SharedPointsAccountId { get; set; }

        public virtual Transaction Transaction { get; set; }

        public virtual MembershipCardTransaction MembershipCardTransaction { get; set; }
    }
}
