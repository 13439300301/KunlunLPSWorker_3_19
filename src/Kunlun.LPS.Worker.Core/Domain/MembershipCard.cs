using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class MembershipCard : BaseEntity
    {
        public long Id { get; set; }

        public long? ProfileId { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string MemberSourceCode { get; set; }

        public DateTime? BindingDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string Password { get; set; }

        public long? MembershipCardBatchId { get; set; }

        public long? MembershipCardBatchDetailId { get; set; }

        public bool IsReport { get; set; }

        public DateTime? RetentionPeriodEndDate { get; set; }

        public bool IsGrowthChanged { get; set; }

        public long? MainMembershipCardId { get; set; }

        public string HotelCode { get; set; }

        public int? CustId { get; set; }
        public DateTime? OpenCardDate { get; set; }
        public MembershipCardStatus Status { get; set; }
        public bool IsFirstStayGiftPointsCalculated { get; set; }
        public DateTime? ChangeTime { get; set; }
        public string CardFaceNumber { get; set; }

        public virtual ICollection<MembershipCardTransaction> MembershipCardTransactions { get; set; }
    }
}
