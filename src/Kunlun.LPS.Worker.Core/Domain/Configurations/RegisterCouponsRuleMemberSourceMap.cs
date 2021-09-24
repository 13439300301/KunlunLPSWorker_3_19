namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class RegisterCouponsRuleMemberSourceMap : ConfigurationBase
    {
        public long Id { get; set; }

        public long RegisterCouponsRuleId { get; set; }

        public string MemberSource { get; set; }
    }
}
