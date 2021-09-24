using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    class RegisterCouponsRuleMemberSourceMapMap : IEntityTypeConfiguration<RegisterCouponsRuleMemberSourceMap>
    {
        public void Configure(EntityTypeBuilder<RegisterCouponsRuleMemberSourceMap> builder)
        {
            builder.ToTable("LPS_RegisterCouponsRule_MemberSource_Map");

            builder.HasKey(c => c.Id);
            builder.Ignore(c => c.Name);
            builder.Ignore(c => c.Code);
        }
    }
}
