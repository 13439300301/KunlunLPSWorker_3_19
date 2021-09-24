using Kunlun.LPS.Worker.Core.Domain.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 账户
    /// </summary>
    public class Account : BaseVersionedEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 该账户所属的会员卡 Id
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 该账户对应的账户类型 Id
        /// </summary>
        public long MembershipCardAccountId { get; set; }

        /// <summary>
        /// 账户类型枚举（0成长值，1积分，2卡值）
        /// </summary>
        public MembershipCardAccountType AccountType { get; set; }

        /// <summary>
        /// 该账户内值的总和
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 用来临时存储优先级的额外属性，不属于数据库级别的
        /// </summary>
        [NotMapped]
        public int? PaymentPriority { get; set; }

        /// <summary>
        ///  透支额度(3.18)
        /// </summary>
        public decimal CreditLine { get; set; }

        /// <summary>
        /// 共享积分账户定义Id
        /// </summary>
        public long? SharedPointsAccountId { get; set; }


    }
}
