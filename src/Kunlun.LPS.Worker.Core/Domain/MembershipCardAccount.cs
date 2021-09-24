using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 账户定义
    /// </summary>
    public class MembershipCardAccount : ConfigurationBase
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 与之关联的卡类型 Id
        /// </summary>
        public long MembershipCardTypeId { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public MembershipCardAccountType Type { get; set; }

        public MembershipCardAccountStoredValueAccountType StoredValueAccountType { get; set; }

        /// <summary>
        /// 账户描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 透支额度
        /// </summary>
        public decimal CreditLine { get; set; }

        /// <summary>
        /// 共享积分账户定义Id
        /// </summary>
        public long? SharedPointsAccountId { get; set; }

    }
}
