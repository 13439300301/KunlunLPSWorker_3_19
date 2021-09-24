using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    class RegisterCouponsRuleMap : IEntityTypeConfiguration<RegisterCouponsRule>
    {
        public void Configure(EntityTypeBuilder<RegisterCouponsRule> builder)
        {
            builder.ToTable("LPS_RegisterCouponsRule");

            builder.HasKey(c => c.Id);
            builder.Ignore(c => c.Name);
            builder.Ignore(c => c.Code);
        }
    }
}
